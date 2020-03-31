using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Save_the_Humans
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        bool humanCaptured = false ;


        public MainWindow()
        {
            InitializeComponent();
            //omzetten van functie van DispatchTimer naar een handler voor de methodes
            enemyTimer.Tick += enemyTimer_Tick;
            //bepaald hoe snel de vijanden verschijnen
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            //omzetten van functie naar een handler voor een event
            targetTimer.Tick += targetTimer_Tick;
            //snelheid progressbar
            targetTimer.Interval = TimeSpan.FromSeconds(.3);
        }

        private void targetTimer_Tick(object sender, EventArgs e)
        {
            //code die de progressbar laat oplopen en ook het het spel laat weten dat het spel geeindigt moeten worden
            //let op!!! de snelheid is niet hier bepaald maar hierboven in de code targettimer.Interval
            progressBar.Value += 1;
            if (progressBar.Value >= progressBar.Maximum)
                EndTheGame();
        }

        private void EndTheGame()
        {
            //wat er moet gebeuren als het spel stopt.
            if (!playArea.Children.Contains(gameOverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                playArea.Children.Add(gameOverText);
            }
        }

        private void enemyTimer_Tick(object sender, EventArgs e)
        {
            // code voor het animeren van de vijanden als je op de startknop klikt
            AddEnemy();
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StartGame()
        {
            //alles wat moet gebeuren als je op de startbutton klikt
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            playArea.Children.Clear();
            playArea.Children.Add(target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();

        }

        /// <summary>
        /// test
        /// </summary>
        private void AddEnemy()
        {
            //code voor the methode addEnemy (hoort bij startbutton)
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(enemy);

            enemy.MouseEnter += enemy_MouseEnter;
        }

        private void enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
                EndTheGame();
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
         {
            //methode die wordt gebruikt in AddEnemy
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            
            DoubleAnimation animation = new DoubleAnimation()
                {
                     From = from,
                     To = to,
                     Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6))),
                };

            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        //code om human te sturen
        private void human_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        //code om human te besturen
        private void human_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;
            }
        }

        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
           //bepaald of human caputered is of niet
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }
    }
}