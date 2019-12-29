using Giselle.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchChat;
using TwitchChat.Commands;

namespace DMAssist.Twitchs
{
    public class TwitchChatManager : IDisposable
    {
        private object StateLock { get; }
        private object ClientLock { get; }

        private Thread RecieveThread;
        private Thread CommThread;
        private TwitchChatClient Client;

        private string PrevChannelName;

        private List<Activity> ActivityQueue;
        private ManualResetEventSlim ActivityResetEvent;
        private ManualResetEventSlim JoinResetEvent;

        public TwitchManagerState State { get; private set; }
        public JoinState JoinState { get; private set; }

        public event EventHandler<CommandEventArgs> CommandRecieve;

        public TwitchChatManager()
        {
            this.StateLock = new object();
            this.ClientLock = new object();

            this.RecieveThread = null;
            this.CommThread = null;
            this.Client = null;
            this.PrevChannelName = null;

            this.ActivityQueue = new List<Activity>();
            this.ActivityResetEvent = new ManualResetEventSlim();
            this.JoinResetEvent = new ManualResetEventSlim();

            this.State = TwitchManagerState.Stopped;
            this.JoinState = JoinState.Parted;
        }

        protected virtual void OnCommandRecieve(CommandEventArgs e)
        {
            this.CommandRecieve?.Invoke(this, e);
        }

        public void AddActivity(Activity activity)
        {
            lock (this.ActivityQueue)
            {
                this.ActivityQueue.Add(activity);
                this.ActivityResetEvent.Set();
            }

        }

        private void UpdateJoinState(JoinState state)
        {
            lock (this.StateLock)
            {
                this.JoinState = state;
                this.JoinResetEvent.Set();
            }

        }

        public void WaitJoinState(JoinState require)
        {
            while (true)
            {
                this.JoinResetEvent.Wait();
                this.JoinResetEvent.Reset();

                if (this.JoinState == require)
                {
                    return;
                }

            }

        }

        public void ChangeChannel(TwitchChatClient client, string channelName)
        {
            var prev = this.PrevChannelName;

            if (prev != null)
            {
                this.UpdateJoinState(JoinState.Parting);
                client.Send(new CommandPart() { Channel = CommandJoin.ChannelPrefix + prev });
                this.WaitJoinState(JoinState.Parted);
            }

            this.PrevChannelName = channelName;

            this.UpdateJoinState(JoinState.Joining);
            client.Send(new CommandJoin() { Channel = CommandJoin.ChannelPrefix + channelName });
            //this.WaitJoinState(JoinState.Joined);
        }

        private void RecieveThreading(object _client)
        {
            var client = _client as TwitchChatClient;

            try
            {
                while (true)
                {
                    if (this.State == TwitchManagerState.Stopping)
                    {
                        return;
                    }

                    var command = client.RecieveCommand();

                    if (command is CommandJoin)
                    {
                        this.UpdateJoinState(JoinState.Joined);
                    }
                    else if (command is CommandPart)
                    {
                        this.UpdateJoinState(JoinState.Parted);
                    }
                    else if (command is CommandPing ping)
                    {
                        client.Send(new CommandPong() { Value = ping.Value });
                    }

                    this.OnCommandRecieve(new CommandEventArgs(command));
                }

            }
            catch (ThreadInterruptedException)
            {

            }
            catch (IOException)
            {

            }
            catch (Exception e)
            {
                Program.Instance.ShowCrashMessageBox(e);
            }

        }

        private void CommThreading()
        {
            try
            {
                while (true)
                {
                    if (this.State == TwitchManagerState.Stopping)
                    {
                        break;
                    }

                    try
                    {
                        TwitchChatClient client = null;

                        lock (this.ClientLock)
                        {
                            client = this.Client = new TwitchChatClient();
                            client.Type = ProtocolType.IRC;
                            client.Capabilities.Add(KnownCapabilities.Tags);
                            client.Capabilities.Add(KnownCapabilities.Commands);
                            client.Nick = "justinfan69740";
                        }

                        client.Connect();

                        lock (this.StateLock)
                        {
                            this.State = TwitchManagerState.Connected;
                            this.ActivityResetEvent.Set();
                        }

                        this.RecieveThread = new Thread(this.RecieveThreading);
                        this.RecieveThread.Start(client);

                        this.ProcessActivity(client);
                    }
                    catch (IOException)
                    {
                        this.State = TwitchManagerState.Diconnected;
                    }
                    finally
                    {
                        ThreadUtils.InterruptAndJoin(this.RecieveThread);
                        this.RecieveThread = null;
                    }

                }

            }
            catch (ThreadInterruptedException)
            {

            }
            catch (IOException)
            {

            }
            catch (Exception e)
            {
                Program.Instance.ShowCrashMessageBox(e);
            }

        }

        private void ProcessActivity(TwitchChatClient client)
        {
            while (true)
            {
                this.ActivityResetEvent.Wait();
                this.ActivityResetEvent.Reset();

                Activity[] array = null;

                lock (this.ActivityQueue)
                {
                    array = this.ActivityQueue.ToArray();
                    this.ActivityQueue.Clear();
                }

                foreach (var activity in array)
                {
                    activity.Act(this, client);
                }

            }

        }

        public void Start()
        {
            lock (this.StateLock)
            {
                this.Stop();

                this.State = TwitchManagerState.Starting;

                this.CommThread = new Thread(this.CommThreading);
                this.CommThread.Start();
            }

        }

        public void Stop()
        {
            lock (this.StateLock)
            {
                try
                {
                    this.State = TwitchManagerState.Stopping;

                    lock (this.ClientLock)
                    {
                        ObjectUtils.DisposeQuietly(this.Client);
                        this.Client = null;
                    }

                    ThreadUtils.InterruptAndJoin(this.CommThread);
                    this.CommThread = null;

                    ThreadUtils.InterruptAndJoin(this.RecieveThread);
                    this.RecieveThread = null;
                }
                finally
                {
                    this.State = TwitchManagerState.Stopped;
                }

            }

        }

        ~TwitchChatManager()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();

            ObjectUtils.DisposeQuietly(this.ActivityResetEvent);
            this.ActivityResetEvent = null;

            ObjectUtils.DisposeQuietly(this.JoinResetEvent);
            this.JoinResetEvent = null;

        }

    }

}
