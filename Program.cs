using System;
using System.Threading;
using System.Collections.Generic;

using Terminal.Gui;

namespace start3
{
    public enum ForksState
    {
        in_usage,
        not_in_usage
    }
    public enum PhilosopherState
    {
        thinking_status,
        eating_status,
        hungry_status
    }

    class Philosopher
    {
        Thread threadAction;
        int posX;
        int posY;
        String name;

        public Philosopher(int X, int Y, String philosopherName)
        {
            posX = X;
            posY = Y;
            name = philosopherName;
        }
        public void setThread(Thread passed)
        {
            threadAction = passed;
        }
        public Thread getThread()
        {
            return threadAction;
        }
        public int getPosX()
        {
            return posX;
        }
        public int getPosY()
        {
            return posY;
        }
        public String getName()
        {
            return name;
        }
    }
    class Program
    {
        List<Philosopher> philosophers = new List<Philosopher>();
        static SemaphoreSlim waiter = new SemaphoreSlim(1);
        static Random rand = new Random();
        static Object randLock = new Object();

        //GUI DATA TO UPDATE
        static int[] pb_data = new int[5];
        static Mutex[] pb_data_mutex = new Mutex[5];

        static PhilosopherState[] states = new PhilosopherState[5];
        static Mutex[] states_mutex = new Mutex[5];

        static bool[] forksUsage = new bool[5];
        static Mutex[] forksUsageMutex = new Mutex[5];

        static List<SemaphoreSlim> forksS = new List<SemaphoreSlim>();
        static int philosopherNumber = 5;

        static int[] hungryArray = new int[5];
        //private const int numIterations = 10;
        static private bool programRunning = true;

        public Program()
        {
            for (int i = 0; i < philosopherNumber; ++i)
            {
                pb_data[i] = 0;
                pb_data_mutex[i] = new Mutex();
                states[i] = PhilosopherState.hungry_status;
                states_mutex[i] = new Mutex();

                //state labels for froks
                forksUsage[i] = false;
                forksUsageMutex[i] = new Mutex();
                hungryArray[i] = i;
                //mut.WaitOne();
                //mut.ReleaseMutex();
            }
        }

        public void ProgramGUI()
        {
            StartThreads();
            Application.Init();
            var top = Application.Top;
            // Creates a menubar f9
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_F9 Menu", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { top.Running = false; }),
                    new MenuItem ("_Stopthreads", "", () => { programRunning = false; }),
                }),
            });
            top.Add(menu);

            //Simulation window
            var win = new Window("PhilosophersProblem") { X = 0, Y = 1, Width = Dim.Fill(), Height = Dim.Fill() };
            //Adding philosophers names (labels)
            var philosopherLabel_0 = new Label(
                    philosophers[0].getName())
            {
                X = philosophers[0].getPosX(),
                Y = philosophers[0].getPosY()
            };
            var philosopherLabel_1 = new Label(
                    philosophers[1].getName())
            {
                X = philosophers[1].getPosX(),
                Y = philosophers[1].getPosY()
            };
            var philosopherLabel_2 = new Label(
                    philosophers[2].getName())
            {
                X = philosophers[2].getPosX(),
                Y = philosophers[2].getPosY()
            };
            var philosopherLabel_3 = new Label(
                    philosophers[3].getName())
            {
                X = philosophers[3].getPosX(),
                Y = philosophers[3].getPosY()
            };
            var philosopherLabel_4 = new Label(
                    philosophers[4].getName())
            {
                X = philosophers[4].getPosX(),
                Y = philosophers[4].getPosY()
            };
            win.Add(philosopherLabel_0);
            win.Add(philosopherLabel_1);
            win.Add(philosopherLabel_2);
            win.Add(philosopherLabel_3);
            win.Add(philosopherLabel_4);
            //Adding status labels (phillosopher state)
            var philosopherStatus_0 =
                new Label("Status 0: thinking?")
                {
                    X = philosophers[0].getPosX(),
                    Y = philosophers[0].getPosY() + 1
                };
            var philosopherStatus_1 =
                new Label("Status 1: thinking?")
                {
                    X = philosophers[1].getPosX(),
                    Y = philosophers[1].getPosY() + 1
                };
            var philosopherStatus_2 =
                new Label("Status 2: thinking?")
                {
                    X = philosophers[2].getPosX(),
                    Y = philosophers[2].getPosY() + 1
                };
            var philosopherStatus_3 =
                new Label("Status 3: thinking?")
                {
                    X = philosophers[3].getPosX(),
                    Y = philosophers[3].getPosY() + 1
                };
            var philosopherStatus_4 =
                new Label("Status 4: thinking?")
                {
                    X = philosophers[4].getPosX(),
                    Y = philosophers[4].getPosY() + 1
                };
            win.Add(philosopherStatus_0);
            win.Add(philosopherStatus_1);
            win.Add(philosopherStatus_2);
            win.Add(philosopherStatus_3);
            win.Add(philosopherStatus_4);
            //Adding progressBars
            ProgressBar pb_0 = customPB(philosophers[0].getPosX(), philosophers[0].getPosY());
            ProgressBar pb_1 = customPB(philosophers[1].getPosX(), philosophers[1].getPosY());
            ProgressBar pb_2 = customPB(philosophers[2].getPosX(), philosophers[2].getPosY());
            ProgressBar pb_3 = customPB(philosophers[3].getPosX(), philosophers[3].getPosY());
            ProgressBar pb_4 = customPB(philosophers[4].getPosX(), philosophers[4].getPosY());
            win.Add(pb_0);
            win.Add(pb_1);
            win.Add(pb_2);
            win.Add(pb_3);
            win.Add(pb_4);

            var forkLabel_0 = new Label(
                    "not in usage")
            {
                X = philosophers[4].getPosX(),
                Y = philosophers[1].getPosY() - 1
            };
            var forkLabel_1 = new Label(
                     "not in usage")
            {
                X = philosophers[1].getPosX(),
                Y = philosophers[1].getPosY() - 1
            };
            var forkLabel_2 = new Label(
                     "not in usage")
            {
                X = philosophers[2].getPosX(),
                Y = philosophers[2].getPosY() - 1
            };
            var forkLabel_3 = new Label(
                     "not in usage")
            {
                X = philosophers[0].getPosX(),
                Y = philosophers[3].getPosY() + 3
            };
            var forkLabel_4 = new Label(
                    "not in usage")
            {
                X = philosophers[4].getPosX(),
                Y = philosophers[3].getPosY() - 1
            };
            win.Add(forkLabel_0);
            win.Add(forkLabel_1);
            win.Add(forkLabel_2);
            win.Add(forkLabel_3);
            win.Add(forkLabel_4);

            //TODO, main loop
            //pb_1.Fraction = (Single)((Single)47 / (Single)100);
            bool timer(MainLoop caller)
            {
                //get dangerous data from???               
                updateInfo(0, pb_0, philosopherStatus_0);
                updateInfo(1, pb_1, philosopherStatus_1);
                updateInfo(2, pb_2, philosopherStatus_2);
                updateInfo(3, pb_3, philosopherStatus_3);
                updateInfo(4, pb_4, philosopherStatus_4);

                updateForks(0, forkLabel_0);
                updateForks(1, forkLabel_1);
                updateForks(2, forkLabel_2);
                updateForks(3, forkLabel_3);
                updateForks(4, forkLabel_4);
                return true;
            }
            Application.MainLoop.AddTimeout(TimeSpan.FromMilliseconds(300), timer);


            top.Add(win);
            Application.Run();
        }
        private void updateForks(int index, Label forkLabel)
        {
            forksUsageMutex[index].WaitOne();
            if (forksUsage[index])
            {
                forkLabel.Text = "in usage";
            }
            else
            {
                forkLabel.Text = "not in usage";
            }
            forksUsageMutex[index].ReleaseMutex();
        }

        private void updateInfo(int index, ProgressBar pb, Label philosopherStatus)
        {
            pb_data_mutex[index].WaitOne();
            pb.Fraction = (Single)((Single)pb_data[index] / (Single)100);
            pb_data_mutex[index].ReleaseMutex();

            states_mutex[index].WaitOne();
            if (states[index] == PhilosopherState.eating_status)
            {
                philosopherStatus.Text = "eating";
                pb.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black);
            }
            else
            {
                if (states[index] == PhilosopherState.thinking_status)
                {
                    philosopherStatus.Text = "thinking";
                    pb.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black);
                }
                else
                {//hungry status
                    philosopherStatus.Text = "hungry";
                    pb.ColorScheme.Normal = new Terminal.Gui.Attribute(Color.Black, Color.BrightRed);
                }
            }
            states_mutex[index].ReleaseMutex();
        }

        static private void thinking()
        {
            int temp = 0;
            lock (randLock)
            {
                temp = rand.Next(2500, 3500);
            }
            //Console.WriteLine("{0} zaczal jesc", Thread.CurrentThread.Name);
            //pb_1.Fraction = (Single)((Single)47 / (Single)100);

            states_mutex[indexOfCurrThread()].WaitOne();
            states[indexOfCurrThread()] = PhilosopherState.thinking_status;
            states_mutex[indexOfCurrThread()].ReleaseMutex();

            for (int i = 0; i < 10; i++)
            {
                pb_data_mutex[indexOfCurrThread()].WaitOne();
                pb_data[indexOfCurrThread()] += 10;
                pb_data_mutex[indexOfCurrThread()].ReleaseMutex();
                Thread.Sleep(temp / 10);
            }
            pb_data_mutex[indexOfCurrThread()].WaitOne();
            pb_data[indexOfCurrThread()] = 0;
            pb_data_mutex[indexOfCurrThread()].ReleaseMutex();

            states_mutex[indexOfCurrThread()].WaitOne();
            states[indexOfCurrThread()] = PhilosopherState.hungry_status;
            states_mutex[indexOfCurrThread()].ReleaseMutex();
        }
        static private void eating()
        {
            forksUsageMutex[getLeftForkId()].WaitOne();
            forksUsage[getLeftForkId()] = true;
            forksUsageMutex[getLeftForkId()].ReleaseMutex();
            forksUsageMutex[getRightForkId()].WaitOne();
            forksUsage[getRightForkId()] = true;
            forksUsageMutex[getRightForkId()].ReleaseMutex();

            int temp = 0;
            lock (randLock)
            {
                temp = rand.Next(2500, 3500);
            }
            //Console.WriteLine("{0} zaczal jesc", Thread.CurrentThread.Name);
            //pb_1.Fraction = (Single)((Single)47 / (Single)100);

            states_mutex[indexOfCurrThread()].WaitOne();
            states[indexOfCurrThread()] = PhilosopherState.eating_status;
            states_mutex[indexOfCurrThread()].ReleaseMutex();

            for (int i = 0; i < 10; i++)
            {
                pb_data_mutex[indexOfCurrThread()].WaitOne();
                pb_data[indexOfCurrThread()] += 10;
                pb_data_mutex[indexOfCurrThread()].ReleaseMutex();
                Thread.Sleep(temp / 10);
            }
            pb_data_mutex[indexOfCurrThread()].WaitOne();
            pb_data[indexOfCurrThread()] = 0;
            pb_data_mutex[indexOfCurrThread()].ReleaseMutex();

            forksUsageMutex[getLeftForkId()].WaitOne();
            forksUsage[getLeftForkId()] = false;
            forksUsageMutex[getLeftForkId()].ReleaseMutex();
            forksUsageMutex[getRightForkId()].WaitOne();
            forksUsage[getRightForkId()] = false;
            forksUsageMutex[getRightForkId()].ReleaseMutex();
        }
        //TODO: remove this, change evrywhere getPhilosopherId()
        static private int indexOfCurrThread()
        {
            return getPhilosopherId();
        }

        private void EndThreads()
        {
            for (int i = 0; i < philosopherNumber; ++i)
            {
                philosophers[i].getThread().Join();
            }
        }
        private static ProgressBar customPB(int posX, int posY)
        {
            ProgressBar pb = new ProgressBar()
            {
                X = posX,
                Y = posY + 2,
                Width = 25,
                Height = 1,
                ColorScheme = new ColorScheme()
                {
                    Normal = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black)
                },//Colors.Error,
            };
            return pb;
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            //program.StartThreads();
            program.ProgramGUI();


            //endThreads joins all philosophers threads
            //program.EndThreads();
            Console.ReadKey();
        }



        //https://docs.microsoft.com/pl-pl/dotnet/api/system.threading.mutex?view=net-5.0
        //TODO: pass "mutex" via argument
        //Tworzy i uruchamia kolejne threads
        //Tworzy mutexy forks
        private void StartThreads()
        {
            philosophers.Add(new Philosopher(16, 0, "Philosopher 0"));
            philosophers.Add(new Philosopher(32, 4, "Philosopher 1"));
            philosophers.Add(new Philosopher(32, 9, "Philosopher 2"));
            philosophers.Add(new Philosopher(0, 9, "Philosopher 3"));
            philosophers.Add(new Philosopher(0, 4, "Philosopher 4"));
            for (int i = 0; i < philosopherNumber; i++)
            {
                SemaphoreSlim smSlim = new SemaphoreSlim(1);
                forksS.Add(smSlim);

                Thread newPhilosopherThread = new Thread(new ThreadStart(ThreadProc));
                newPhilosopherThread.Name = String.Format("{0}", i);
                newPhilosopherThread.Start();
                philosophers[i].setThread(newPhilosopherThread);
            }
        }

        //TODO : ThreadProc should be complete 1 cycle of philosophers life routine
        // THINKING->HUNGRY->EATING-> (thinking)
        // implement grabing fork/chopsticks before eating
        // implement arbiter to let selected philosophers eat- prevent deadlock/livelock
        // prevent starvation/hungry- dont let eat neigbhours of the most starved philosopher
        private static void ThreadProc()
        {
            while (programRunning)
            {
                //Waiter instructions here!!!
                waiter.Wait();//waiter is listening one person at time
                var leftForkIndex = getLeftForkId();//info from current.Thread
                var rightForkIndex = getRightForkId();
                if (forksS[leftForkIndex].CurrentCount == 1 && forksS[rightForkIndex].CurrentCount == 1)
                {
                    //Waiter can let this philosopher eat
                    //rozwiazanie zagłodzenia?
                    if (hungryArray[(getPhilosopherId() + 1) % 5] == 4 || hungryArray[(getPhilosopherId() + 9) % 5] == 4)
                    {
                        //refuse to eat() bcs of starving neighbhour
                        waiter.Release();
                        continue;
                    }
                    else
                    {
                        //if he is the most hungry
                        if (hungryArray[getPhilosopherId()] == 4)
                        {
                            //the most hungry
                            hungryArray[0] = (hungryArray[0] + 1) % 5;
                            hungryArray[1] = (hungryArray[1] + 1) % 5;
                            hungryArray[2] = (hungryArray[2] + 1) % 5;
                            hungryArray[3] = (hungryArray[3] + 1) % 5;
                            hungryArray[4] = (hungryArray[4] + 1) % 5;
                        }
                        else
                        {
                            //not the most hungry...
                            //let others go upwards
                            for (int i = 0; i < 5; i++)
                            {
                                if (hungryArray[i] < hungryArray[getPhilosopherId()])
                                {
                                    hungryArray[i] += 1;
                                }
                            }
                            //put him at the end
                            hungryArray[getPhilosopherId()] = 0;
                        }
                        forksS[leftForkIndex].Wait();
                        forksS[rightForkIndex].Wait();
                    }
                }
                else
                {
                    //after checking 2 forks, waiter tells that philosopher cant eat
                    waiter.Release();
                    continue;
                }
                waiter.Release();//waiter can serve next philosopher
                //this philosopher can eat
                eating();
                //put down forks- u can put down them without asking about permission
                forksS[leftForkIndex].Release();
                forksS[rightForkIndex].Release();
                thinking();
            }
        }

        static int getPhilosopherId()
        {
            return Convert.ToInt32(Thread.CurrentThread.Name);
        }
        static int getRightForkId()
        {
            return getPhilosopherId();
        }
        static int getLeftForkId()
        {
            return (getPhilosopherId() + 1) % philosopherNumber;
        }
    }
}
