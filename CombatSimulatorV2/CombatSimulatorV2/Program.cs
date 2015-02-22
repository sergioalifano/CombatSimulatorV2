using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatSimulatorV2
{
    class Program
    {       
        static void Main(string[] args)
        {
            Game game = new Game();
            game.PlayGame();

            Console.ReadKey();
        }       
    }


    #region "Actor"
    public abstract class Actor
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public bool IsAlive
        {
            get
            {
                return this.HP > 0;
            }
        }
        public Random RNG{ get; set; }

        public Actor(string name,int hp)
        {
            this.Name = name;
            this.HP = hp;
            this.RNG = new Random();
        }

        public virtual void DoAttack(Actor actor) { }
    }
    #endregion


    #region "Enemy"
    public class Enemy : Actor
    {
        public Enemy(string name, int hp) : base(name, hp) {}
        public override void DoAttack(Actor actor)
        {
            //if the dragon hit
            if (RNG.Next(0, 11) <= 8)
            {
                int attackResult = RNG.Next(5, 16);
                actor.HP -= attackResult;
                Console.SetCursorPosition(41, 26);

                Console.Write("Ahahah {0} HP less for you!!          ",attackResult);
            }
            else
            {
                Console.SetCursorPosition(41, 26);
                Console.Write("You're so lucky! I missed!              ");
            }
            Console.ResetColor();   
        }
    }

    #endregion


    #region "Player"
    public class Player : Actor
    {
        //index for the foreground color
        static int colorIndex=0;
        
        enum AttackType
        {           
            sword,
            magic,
            heal, 
            invalidChoice
        }

        public Player(string name, int hp) : base(name, hp) { }

        
        public override void DoAttack(Actor actor)
        {
            //List of color
            List<string> colors = new List<string> { "Blue", "Magenta" };

            AttackType attack = ChooseAttack();
            if (attack == AttackType.sword)
            {
                int damage;
                //if it's in the 70% of the positive attack 
                if (RNG.Next(0, 11) <= 7)
                {
                    //decrement dragon energy
                    damage = RNG.Next(20, 36);
                    actor.HP -= damage;
                    
                    //show message with different color
                    Console.ForegroundColor = ChangeColor(colors, ref colorIndex);
                    Console.SetCursorPosition(0, 25);
                    Console.Write("\nYou hit the dragon!!                     ");                   
                }
            }
            else if (attack == AttackType.magic)
            {                
                actor.HP -= RNG.Next(10, 15);

                //show message with different color
                Console.ForegroundColor = ChangeColor(colors, ref colorIndex);
                Console.SetCursorPosition(0, 25);
                Console.Write("\nGood choice. Magic always strike!        ");
            }
            else if(attack == AttackType.heal)
            {
                //heal energy
                int energyHp = RNG.Next(10, 21);
                this.HP += energyHp;

                //show message with different color
                Console.ForegroundColor = ChangeColor(colors, ref colorIndex);
                Console.SetCursorPosition(0, 25);
                Console.Write("\nYou got {0} HP                           ",energyHp);
            }
            else if (attack == AttackType.invalidChoice)
            {
                Console.ForegroundColor = ChangeColor(colors, ref colorIndex);
                Console.SetCursorPosition(0, 26);
                Console.Write("Oooops. Choose wisely next time            ");
            }
           
        }

        /// <summary>
        /// Pick a color from the list of the colors
        /// </summary>
        /// <param name="colors">List of available colors</param>
        /// <param name="colorIndex">Index of the list of colors</param>
        /// <returns>The color picked</returns>
        static ConsoleColor ChangeColor(List<string> colors, ref int colorIndex)
        {
            colorIndex = colorIndex % colors.Count();

            //pick a color from the list of colors
            string color = colors[colorIndex];

            colorIndex++;

            //return the color as ConsoleColor type
            return (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color);
        }

        private AttackType ChooseAttack()
        {
            Console.SetCursorPosition(0, 18);
            Console.WriteLine("Choose your attack: ");
            Console.WriteLine("1. Attack with a sword\n2. Use the magic and throw a Fireball!\n3. Use the medication\n\n");

            string input;
            if (validateInput(input=Console.ReadLine()))
            {
                switch (int.Parse(input))
                {                   
                    case 1: return AttackType.sword;                                          
                    case 2: return AttackType.magic;                                         
                    case 3: return AttackType.heal;
                    default: return AttackType.invalidChoice;
                }
            }

            //if the choice is not a number
            return AttackType.invalidChoice;
        }

        private bool validateInput(string input)
        {
            int inputNumber;
            //if the input is a number
            if (int.TryParse(input, out inputNumber))
            {
                if (inputNumber > 0 && inputNumber < 4)
                {
                    return true;
                }
            }
            return false;
        }
    }
#endregion


    #region "Game"
    public class Game
    {
        public Player player;
        public Enemy enemy;

        public Game()
        {
            this.player = new Player("Sergio",100);
            this.enemy = new Enemy("Dragon", 200);
        }

        public void DisplayCombatInfo()
        {
            ShowActorEnergy(player);
            ShowActorEnergy(enemy);
        }

        public void PlayGame()
        {
            //Change the console Height to fit the description of the game
            Console.SetWindowSize(Console.WindowWidth, Console.LargestWindowHeight);

            DisplayBanner();
            DisplayDragon();
            DisplayStory();

            Console.Clear();
            DisplayBanner();
            while (this.player.IsAlive && this.enemy.IsAlive)
            {
                DisplayCombatInfo();
                this.player.DoAttack(this.enemy);
                this.enemy.DoAttack(this.player);
            }
            if (this.player.IsAlive)
            {
                DisplayVictory();
            }
            else
            {
                DisplayLose();
            }
        }

        /// <summary>
        /// Print out the logo of the game
        /// </summary>
        static void DisplayBanner()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(@" ██████╗ ██████╗ ███╗   ███╗██████╗  █████╗ ████████╗                    
██╔════╝██╔═══██╗████╗ ████║██╔══██╗██╔══██╗╚══██╔══╝                    
██║     ██║   ██║██╔████╔██║██████╔╝███████║   ██║                       
██║     ██║   ██║██║╚██╔╝██║██╔══██╗██╔══██║   ██║                       
╚██████╗╚██████╔╝██║ ╚═╝ ██║██████╔╝██║  ██║   ██║                       
 ╚═════╝ ╚═════╝ ╚═╝     ╚═╝╚═════╝ ╚═╝  ╚═╝   ╚═╝                       
                                                                         
███████╗██╗███╗   ███╗██╗   ██╗██╗      █████╗ ████████╗ ██████╗ ██████╗ 
██╔════╝██║████╗ ████║██║   ██║██║     ██╔══██╗╚══██╔══╝██╔═══██╗██╔══██╗
███████╗██║██╔████╔██║██║   ██║██║     ███████║   ██║   ██║   ██║██████╔╝
╚════██║██║██║╚██╔╝██║██║   ██║██║     ██╔══██║   ██║   ██║   ██║██╔══██╗
███████║██║██║ ╚═╝ ██║╚██████╔╝███████╗██║  ██║   ██║   ╚██████╔╝██║  ██║
╚══════╝╚═╝╚═╝     ╚═╝ ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝    ╚═════╝ ╚═╝  ╚═╝
                                                                         ");
            Console.ResetColor();
        }

        /// <summary>
        /// Print ouf the dragon
        /// </summary>
        static void DisplayDragon()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"
                  -.            \_|//     |||\\  ~~~~~~::::... /~
               ___-==_       _-~o~  \/    |||  \\            _/~~-
       __---~~~.==~||\=_    -_--~/_-~|-   |\\   \\        _/~
   _-~~     .=~    |  \\-_    '-~7  /-   /  ||    \      /
  ~       .~       |   \\ -_    /  /-   /   ||      \   /
/  ____  /         |     \\ ~-_/  /|- _/   .||       \ /
|~~    ~~|--~~~~--_ \     ~==-/   | \~--===~~        .\
         '         ~-|      /|    |-~\~~       __--~~
                     |-~~-_/ |    |   ~\_   _-~            /\
                          /  \     \__   \/~                \__
                      _--~ _/ | .-~~____--~-/                  ~~==.
                     ((->/~   '.|||' -_|    ~~-/ ,              . _||
                               -_     ~\      ~~---l__i__i__i--~~_/
                                _-~-__   ~)  \--______________--~~
                              //.-~~~-~_--~- |-------~~~~~~~~
                                     //.-~~~--\");
            Console.ResetColor();
            Console.WriteLine();
        }

        /// <summary>
        /// Print out the story 
        /// </summary>
        /// <returns></returns>
        static void DisplayStory()
        {
            string text = @"
You are a brave knight and you must save the princess trapped in the castle. 
To get to her you must first defeat the terrible dragon 
that defends the castle doors.

You have two possible weapons: SWORD and FIREBALLS. 
To heal yourself you can use the MAGIC POTION that 
Merlin the Magician gave you before leaving.

The SWORD is the weapon that inflicts more damages
but it hits only 70% of the times.
The FIREBALL always strikes but does least damage to the dragon.
The MAGIC POTION gives you energy between 10 and 20 HP.

You have initially 100 HP
The dragon has 200 HP

GOOD LUCK!!!";
            foreach (char c in text)
            {
                Console.Write(c);
                System.Threading.Thread.Sleep(1);
            }
            Console.WriteLine("\n\nPress Enter to begin...");
            Console.ReadLine();
        }

        public void ShowActorEnergy(Actor actor)
        {
            string energyLevel = string.Empty;
            //these if are to choose the amount of energy to display and the color of the status bar
            if (actor.HP > 95)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < 10; i++)
                {
                    energyLevel += "██";
                }
            }
            else if (actor.HP > 80)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                for (int i = 0; i < 8; i++)
                {
                    energyLevel += "██";
                }
            }
            else if (actor.HP > 60)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int i = 0; i < 6; i++)
                {
                    energyLevel += "██";
                }

            }
            else if (actor.HP > 40)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                for (int i = 0; i < 4; i++)
                {
                    energyLevel += "██";
                }

            }
            else if (actor.HP > 20)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                for (int i = 0; i < 2; i++)
                {
                    energyLevel += "██";
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                energyLevel += "██";
            }
            if(actor == player)
            {
                Console.SetCursorPosition(0, 15);
                Console.Write("{0}: " + energyLevel + " {1}HP          ", actor.Name, actor.HP);
                Console.ResetColor();
            }

            else
            {
                Console.SetCursorPosition(40, 15);
                Console.Write("{0}: " + energyLevel + " {1}HP          ", actor.Name, actor.HP);
                Console.ResetColor();
            }
               
        }

        /// <summary>
        /// Print out a castle for the victory
        /// </summary>
        static void DisplayVictory()
        {
            Console.Clear();
            Console.WriteLine(@"
                                       /\
                                      /`:\
                                     /`'`:\
                                    /`'`'`:\
                                   /`'`'`'`:\
                                  /`'`'`'`'`:\
                                   |`'`'`'`:|
     _ _  _  _  _                  |] ,-.  :|_  _  _  _
    ||| || || || |                 |  |_| ||| || || || |
    |`' `' `' `'.|                 | _'=' |`' `' `' `'.|
    :          .:;                 |'-'   :          .:;
     \-..____..:/  _  _  _  _  _  _| _  _'-\-..____..:/
      :--------:_,' || || || || || || || `.::--------:
      |]     .:|:.  `' `'_`' `' `' `' `'    | '-'  .:|
      |  ,-. .[|:._     '-' ____     ___    |   ,-.'-|
      |  | | .:|'--'_     ,'____`.  '---'   |   | |.:|
      |  |_| .:|:.'--' ()/,| |`|`.\()   __  |   |_|.:|
      |  '=' .:|:.     |::_|_|_|\|::   '--' |  _'='.:|
      | __   .:|:.     ;||-,-,-,-,|;        | '--' .:|
      |'--'  .:|:. _  ; ||       |:|        |      .:|
      |      .:|:.'-':  ||       |;|     _  |]     _:|
      |      '-|:.   ;  ||       :||    '-' |     '--|
      |  _   .:|].  ;   ||       ;||]       |   _  .:|
      | '-'  .:|:. :   [||      ;|||        |  '-' .:|
  ,', ;._____.::-- ;---->'-,--,:-'<'--------;._____.::.`.
 ((  (          )_;___,' ,' ,  ; //________(          ) ))
  `. _`--------' : -,' ' , ' '; //-       _ `--------' ,'
       __  .--'  ;,' ,'  ,  ': //    -.._    __  _.-  -
   `-   --    _ ;',' ,'  ,' ,;/_  -.       ---    _,
       _,.   /-:,_,_,_,_,_,_(/:-\   ,     ,.    _
     -'   `-'--'-'-'-'-'-'-'-''--'-' `-'`'  `'`' `-");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"__   __                                    _ 
\ \ / /                                   | |
 \ V /___  _   _   ___  __ ___   _____  __| |
  \ // _ \| | | | / __|/ _` \ \ / / _ \/ _` |
  | | (_) | |_| | \__ \ (_| |\ V /  __/ (_| |
  \_/\___/ \__,_| |___/\__,_| \_/ \___|\__,_|
                                             
                                             ");
            Console.WriteLine(@" _   _                       _                           _ _ _ 
| | | |                     (_)                         | | | |
| |_| |__   ___   _ __  _ __ _ _ __   ___ ___  ___ ___  | | | |
| __| '_ \ / _ \ | '_ \| '__| | '_ \ / __/ _ \/ __/ __| | | | |
| |_| | | |  __/ | |_) | |  | | | | | (_|  __/\__ \__ \ |_|_|_|
 \__|_| |_|\___| | .__/|_|  |_|_| |_|\___\___||___/___/ (_|_|_)
                 | |                                           
                 |_|                                           ");
            Console.ReadKey();
        }


        /// <summary>
        /// Print out the lose with an animated dragon
        /// </summary>
        static void DisplayLose()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(@"
                                                |===-~___                _,-'
                 -==\\                         `//~\\   ~~~~`---.___.-~~
             ______-==|                         | |  \\           _-~`
       __--~~~  ,-/-==\\                        | |   `\        ,'
    _-~       /'    |  \\                      / /      \      /
  .'        /       |   \\                   /' /        \   /'
 /  ____  /         |    \`\.__/-~~ ~ \ _ _/'  /          \/'
/-'~    ~~~~~---__  |     ~-/~         ( )   /'        _--~`
                  \_|      /        _)   ;  ),   __--~~
                    '~~--_/      _-~/-  / \   '-~ \
                   {\__--_/}    / \\_>- )<__\      \
                   /'   (_/  _-~  | |__>--<__|      | 
                  |0  0 _/) )-~     | |__>--<__|      |
                  / /~ ,_/       / /__>---<__/      |  
                 o o _//        /-~_>---<__-~      /
                 (^(~          /~_>---<__-      _-~              
                ,/|           /__>--<__/     _-~                 
             ,//('(          |__>--<__|     /                  .----_ 
            ( ( '))          |__>--<__|    |                 /' _---_~\
         `-)) )) (           |__>--<__|    |               /'  /     ~\`\
        ,/,'//( (             \__>--<__\    \            /'  //        ||
      ,( ( ((, ))              ~-__>--<_~-_  ~--____---~' _/'/        /'
    `~/  )` ) ,/|                 ~-_~>--<_/-__       __-~ _/ 
  ._-~//( )/ )) `                    ~~-'_/_/ /~~~~~~~__--~ 
   ;'( ')/ ,)(                              ~~~~~~~~~~ 
  ' ') '( (/");
                Console.WriteLine(@"
     )    )                                
  ( /( ( /(             (                  
  )\()))\())     (      )\            (    
 ((_)\((_)\      )\    ((_) (   (    ))\   
__ ((_) ((_)  _ ((_)    _   )\  )\  /((_)  
\ \ / // _ \ | | | |   | | ((_)((_)(_))    
 \ V /| (_) || |_| |   | |/ _ \(_-</ -_)   
  |_|  \___/  \___/    |_|\___//__/\___|   
                                           ");
                System.Threading.Thread.Sleep(1500);
                Console.Clear();
                Console.WriteLine(@"
                                                |===-~___                _,-'
                 -==\\                         `//~\\   ~~~~`---.___.-~~
             ______-==|                         | |  \\           _-~`
       __--~~~  ,-/-==\\                        | |   `\        ,'
    _-~       /'    |  \\                      / /      \      /
  .'        /       |   \\                   /' /        \   /'
 /  ____  /         |    \`\.__/-~~ ~ \ _ _/'  /          \/'
/-'~    ~~~~~---__  |     ~-/~         ( )   /'        _--~`
                  \_|      /        _)   ;  ),   __--~~
                    '~~--_/      _-~/-  / \   '-~ \
                   {\__--_/}    / \\_>- )<__\      \
                   /'   (_/  _-~  | |__>--<__|      | 
                  |0  0 _/) )-~     | |__>--<__|      |
                  / /~ ,_/       / /__>---<__/      |  
                 o o _//        /-~_>---<__-~      /
                               /~_>---<__-      _-~              
                              /__>--<__/     _-~                 
                             |__>--<__|     /                  .----_ 
                             |__>--<__|    |                 /' _---_~\
                             |__>--<__|    |               /'  /     ~\`\
                              \__>--<__\    \            /'  //        ||
                               ~-__>--<_~-_  ~--____---~' _/'/        /'
                                  ~-_~>--<_/-__       __-~ _/ 
                                     ~~-'_/_/ /~~~~~~~__--~ 
                                            ~~~~~~~~~~");
                Console.WriteLine(@"

     )    )                                
  ( /( ( /(             (                  
  )\()))\())     (      )\            (    
 ((_)\((_)\      )\    ((_) (   (    ))\   
__ ((_) ((_)  _ ((_)    _   )\  )\  /((_)  
\ \ / // _ \ | | | |   | | ((_)((_)(_))    
 \ V /| (_) || |_| |   | |/ _ \(_-</ -_)   
  |_|  \___/  \___/    |_|\___//__/\___|   
                                           ");
                System.Threading.Thread.Sleep(500);
                Console.Clear();
            }
            Console.ReadKey();

        }
    }
#endregion
    
}
