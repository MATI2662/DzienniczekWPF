using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Microsoft.VisualBasic;
using System.Data.Sql;
using System.Data.SqlClient;

namespace DzinniczekWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
     
    
    public partial class MainWindow : Window
    {
        public string addedSchool; 
        public string addedClass;
        public string[] addedStudent = new string[4];
        public string[] editedStudent = new string[4];

        Random random = new Random();

        List<Scl> currentSchools = new List<Scl>();
        List<Cls> currentClsses = new List<Cls>();
        List<Std> currentStudents = new List<Std>();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Schools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Classes.Items.Clear();
            Students.Items.Clear();
            StudentData.Content = "";

            currentClsses.Clear();
            if (Schools.SelectedIndex > -1) { 
                foreach(Cls cls in currentSchools[Schools.SelectedIndex].classes)
                {
                    currentClsses.Add(cls);

                    Classes.Items.Add(cls.name);
                }
            }

            int index = (Schools.SelectedIndex > 0) ? Schools.SelectedIndex : 0;
            if (currentSchools.Count() > 0)
            {
                Scl scl = currentSchools[index];

                StudentData.Content += "Nazwa: " + scl.name + "\n";
                StudentData.Content += "ID: " + scl.id + "\n";
                StudentData.Content += "Ilość uczniów: " + scl.CountAllStudents() + "\n";
            }
        }

        private void Classes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Students.Items.Clear();
            StudentData.Content = "";

            currentStudents.Clear();

            int i = 1;

            if(currentClsses.Count > 0)
            {
                int index = (Classes.SelectedIndex > 0) ? Classes.SelectedIndex : 0;

                foreach (Std std in currentClsses[index].students)
                {
                    currentStudents.Add(std);

                    Students.Items.Add(std.name + " " + std.surname);
                }

                Cls cls = currentClsses[index];

                StudentData.Content += "Nazwa: " + cls.name + "\n";
                StudentData.Content += "ID: " + cls.id + "\n";
                StudentData.Content += "Ilość uczniów: " + cls.students.Count + "\n";
                StudentData.Content += "Procent mężczyzn: " + cls.CalculatePercents().manPercent + "%\n";
                StudentData.Content += "Procent kobiet: " + cls.CalculatePercents().womanPercent + "%\n";
                StudentData.Content += "Średnia inteligencja: " + cls.AverageInteligence() + "\n";
                StudentData.Content += "Średnia siła: " + cls.AverageStrenght() + "\n";
            }
        }

        private void Students_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StudentData.Content = "";

            if (currentClsses.Count > 0)
            {
                int index = (Students.SelectedIndex > 0) ? Students.SelectedIndex : 0;
                if (currentStudents.Count() > 0) { 
                    Std std = currentStudents[index];

                    StudentData.Content += std.name + " " + std.surname + "\n";
                    StudentData.Content += "ID: " + std.id + "\n";
                    StudentData.Content += "Płeć: " + (std.sex ? "Mężczyzna" : "Kobieta") + "\n";
                    StudentData.Content += "Wiek: " + std.age + "\n";
                    StudentData.Content += "Inteligencja: " + std.inteligence + "\n";
                    StudentData.Content += "Siła: " + std.strenght + "\n";
                }
            }
        }

        private void AddSchool_Click(object sender, RoutedEventArgs e)
        {
            AddSchool addSchool = new AddSchool();
            addSchool.Show();
        }

        private void Main_Closed(object sender, EventArgs e)
        {
            saveSchools();
        }

        private void Main_Loaded(object sender, EventArgs e)
        {
            loadSchools(ref currentSchools);
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            //Adding School - checking if some school was sent
            if (addedSchool != "" && addedSchool != null)
            {
                currentSchools.Add(new Scl(currentSchools.Last().id + 1, addedSchool));
                Schools.Items.Add(currentSchools.Last().name);
                addedSchool = "";
            }

            //Adding Class - checking if some class was sent
            if (addedClass != "" && addedClass != null) 
            {
                currentClsses.Add(new Cls(currentClsses.Last().id + 1, addedClass));
                currentSchools[Schools.SelectedIndex].AddClass(new Cls(currentClsses.Last().id, addedClass));
                Classes.Items.Add(currentClsses.Last().name);
                addedClass = "";
            }

            //Adding Student - checking if some student was sent
            if (addedStudent[0] != "" && addedStudent[0] != null && addedStudent[1] != "" && addedStudent[1] != null && addedStudent[2] != "" && addedStudent[2] != null && addedStudent[3] != "" && addedStudent[3] != null) 
            {
                int i = currentClsses[Classes.SelectedIndex].students.Count() + 1;
                currentStudents.Add(new Std(currentStudents.Last().id + 1, addedStudent[0], addedStudent[1], int.Parse(addedStudent[2]), bool.Parse(addedStudent[3]), random.Next(0, 99), random.Next(0, 99)));
                currentClsses[Classes.SelectedIndex].AddStudent(new Std(currentStudents.Last().id, addedStudent[0], addedStudent[1], int.Parse(addedStudent[2]), bool.Parse(addedStudent[3]), random.Next(0, 99), random.Next(0, 99)));
                currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students.Add(new Std(currentStudents.Last().id, addedStudent[0], addedStudent[1], int.Parse(addedStudent[2]), bool.Parse(addedStudent[3]), random.Next(0, 99), random.Next(0, 99)));
                Students.Items.Add(currentStudents.Last().name + " " + currentStudents.Last().surname);
                addedStudent[0] = "";
                addedStudent[1] = "";
                addedStudent[2] = "";
                addedStudent[3] = "";
            }

            //Student Info Update - checking if info was changed
            if (editedStudent[0] != "" && editedStudent[0] != null && editedStudent[1] != "" && editedStudent[1] != null && editedStudent[2] != "" && editedStudent[2] != null && editedStudent[3] != "" && editedStudent[3] != null) 
            {
                currentStudents[Students.SelectedIndex].name = editedStudent[0];
                currentStudents[Students.SelectedIndex].surname = editedStudent[1];
                currentStudents[Students.SelectedIndex].age = int.Parse(editedStudent[2]);
                currentStudents[Students.SelectedIndex].sex = bool.Parse(editedStudent[3]);

                currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students[Students.SelectedIndex].name = editedStudent[0];
                currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students[Students.SelectedIndex].surname = editedStudent[1];
                currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students[Students.SelectedIndex].age = int.Parse(editedStudent[2]);
                currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students[Students.SelectedIndex].sex = bool.Parse(editedStudent[3]);

                StudentData.Content = "";
                StudentData.Content += editedStudent[0] + " " + editedStudent[1] + "\n";
                StudentData.Content += "Płeć: " + (bool.Parse(editedStudent[3]) ? "Mężczyzna" : "Kobieta") + "\n";
                StudentData.Content += "Wiek: " + editedStudent[2] + "\n";
                StudentData.Content += "Inteligencja: " + currentStudents[Students.SelectedIndex].inteligence + "\n";
                StudentData.Content += "Siła: " + currentStudents[Students.SelectedIndex].strenght + "\n";

                editedStudent[0] = "";
                editedStudent[1] = "";
                editedStudent[2] = "";
                editedStudent[3] = "";
            }
        }

        private void AddClass_Click(object sender, RoutedEventArgs e)
        {
            if(Schools.SelectedItem != null)
            {
                AddClass addClass = new AddClass();
                addClass.Show();
            }
        }

        private void AddStudent_Click(object sender, RoutedEventArgs e)
        {
            if (Schools.SelectedItem != null)
            {
                if(Classes.SelectedItem != null)
                {
                    AddStudent addStudent = new AddStudent();
                    addStudent.Show();
                }
            }
            
        }

        private void DeleteStudent_Click(object sender, RoutedEventArgs e)
        {
            if (Schools.SelectedItem != null)
            {
                if (Classes.SelectedItem != null)
                {
                    if (Students.SelectedItem != null)
                    {
                        int index = Students.SelectedIndex;
                        currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students.RemoveAt(Students.SelectedIndex);
                        currentStudents.RemoveAt(Students.SelectedIndex);
                        Students.Items.RemoveAt(Students.SelectedIndex);
                        Students.SelectedIndex = index - 1;
                    }    
                }
            }
            
        }

        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            if (Schools.SelectedItem != null)
            {
                if (Classes.SelectedItem != null)
                {
                    int index = Classes.SelectedIndex;
                    currentSchools[Schools.SelectedIndex].classes[Classes.SelectedIndex].students.Clear();
                    currentSchools[Schools.SelectedIndex].classes.RemoveAt(Classes.SelectedIndex);
                    currentClsses.RemoveAt(Classes.SelectedIndex);
                    Classes.Items.RemoveAt(Classes.SelectedIndex);
                    Classes.SelectedIndex = index - 1;
                }
            }
        }

        private void DeleteSchool_Click(object sender, RoutedEventArgs e)
        {
            if (Schools.SelectedItem != null)
            {
                int index = Schools.SelectedIndex;
                currentSchools[Schools.SelectedIndex].classes.Clear();
                currentSchools.RemoveAt(Schools.SelectedIndex);
                Schools.Items.RemoveAt(Schools.SelectedIndex);
                Schools.SelectedIndex = index - 1;
            }
        }

        private void EditStudent_Click(object sender, RoutedEventArgs e)
        {
            if (Schools.SelectedItem != null) 
            {
                if (Classes.SelectedItem != null) 
                {
                    if (Students.SelectedItem != null) 
                    {
                        EditStudent editStudent = new EditStudent();
                        editStudent.studentInfo[0] = currentStudents[Students.SelectedIndex].name;
                        editStudent.studentInfo[1] = currentStudents[Students.SelectedIndex].surname;
                        editStudent.studentInfo[2] = currentStudents[Students.SelectedIndex].age.ToString();
                        editStudent.studentInfo[3] = currentStudents[Students.SelectedIndex].sex.ToString();

                        editStudent.Show();
                    }
                }
            }
        }

        private void saveSchools()
        {
            string fileName = @"Schools.json";

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            string jsonstring = JsonSerializer.Serialize(currentSchools, options);

            if (jsonstring == "")
            {
                File.WriteAllText(fileName, "");
            }
            else
            {
                File.WriteAllText(fileName, jsonstring);
            }
        }
        
        private void loadSchools(ref List<Scl> currentSchools)
        {
            string fileName = @"Schools.json";
            string lines = File.ReadAllText(fileName);
            File.WriteAllText(fileName, "");
            if (lines == null || lines == "")
            {
                Std std0 = new(0, "Irma", "Drozd", 17, false, random.Next(0, 99), random.Next(0,100));
                Std std1 = new(1, "Franciszka", "Orłowska", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std2 = new(2, "Witalis", "Mazur", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std3 = new(3, "Waleria", "Michalak", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std4 = new(4, "Dina", "Wąsik", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std5 = new(5, "Porfiriusz", "Lewandowski", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std6 = new(6, "Łazarz", "Karaś", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std7 = new(7, "Nikodem", "Małecki", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std8 = new(8, "Grzegorz", "Woliński", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std9 = new(9, "Teodora", "Rogowska", 18, false, random.Next(0, 99), random.Next(0, 99));
                Std std10 = new(10, "Oliwier", "Budziński", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std11 = new(11, "Walenty", "Buczyński", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std12 = new(12, "Sylwia", "Wilczek", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std13 = new(13, "Amanda", "Łapiński", 18, false, random.Next(0, 99), random.Next(0, 99));
                Std std14 = new(14, "Władysław", "Walczak", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std15 = new(15, "Gerwazy", "Królikowski", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std16 = new(16, "Korneli", "Szymański", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std17 = new(17, "Helena", "Dobosz", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std18 = new(18, "Seweryn", "Bartnik", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std19 = new(19, "Kornel", "Kowal", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std20 = new(20, "Dawid", "Zięba", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std21 = new(21, "Eudoksja", "Kowalewska", 18, false, random.Next(0, 99), random.Next(0, 99));
                Std std22 = new(22, "Sybilla", "Szczepańska", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std23 = new(23, "Antoni", "Wolak", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std24 = new(24, "Leon", "Jarosz", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std25 = new(25, "Nina", "Adamiak", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std26 = new(26, "Laurenty", "Jabłoński", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std27 = new(27, "Kornel", "Bielecki", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std28 = new(28, "Franciszka", "Wypych", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std29 = new(29, "Nataniel", "Kuchta", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std30 = new(30, "Róża", "Miklaś", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std31 = new(31, "Aurora", "Paszkowska", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std32 = new(32, "Samuel", "Ossowski", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std33 = new(33, "Anastazja", "Urbańska", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std34 = new(34, "Rozalia", "Mazur", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std35 = new(35, "Malwina", "Krzyżanowska", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std36 = new(36, "Konstantyn", "Czekaj", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std37 = new(37, "Angelina", "Lenart", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std38 = new(38, "Katarzyna", "Milewska", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std39 = new(39, "Mikołaj", "Gajewski", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std40 = new(40, "Gerazym", "Ratajczak", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std41 = new(41, "Józefina", "Sawicka", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std42 = new(42, "Reginald", "Kędzierski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std43 = new(43, "Józefina", "Słowik", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std44 = new(44, "Bibiana", "Kita", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std45 = new(45, "Dorota", "Grzelak", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std46 = new(46, "Adrian", "Skoczylas", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std47 = new(47, "Sergiusz", "Pietras", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std48 = new(48, "Krystian", "Lesiak", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std49 = new(49, "Maryna", "Słowik", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std50 = new(50, "Kalistrat", "Mazur", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std51 = new(51, "Patrycy", "Pawlikowski", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std52 = new(52, "Izabela", "Janas", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std53 = new(53, "Lilia", "Bielak", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std54 = new(54, "Aleksy", "Łukaszewski", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std55 = new(55, "Hanna", "Modzelewska", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std56 = new(56, "Ludwik", "Bednarek", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std57 = new(57, "Bazyli", "Czerniak", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std58 = new(58, "Eleonora", "Zajączkowska", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std59 = new(59, "Janusz", "Dudzik", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std60 = new(60, "Agnieszka", "Sokołowska", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std61 = new(61, "Bibiana", "Molenda", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std62 = new(62, "Laurenty", "Siwek", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std63 = new(63, "Korneli", "Kopczyński", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std64 = new(64, "Gertruda", "Kruszewska", 18, false, random.Next(0, 99), random.Next(0, 99));
                Std std65 = new(65, "Gwido", "Stawicki", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std66 = new(66, "Zoe", "Rogalska", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std67 = new(67, "Malwina", "Bartkowiak", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std68 = new(68, "Tamara", "Głowacka", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std69 = new(69, "Terencjusz", "Mróz", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std70 = new(70, "Nadzieja", "Kujawa", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std71 = new(71, "Zachary", "Błaszczyk", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std72 = new(72, "Tomasz", "Wojciechowski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std73 = new(73, "Dariusz", "Kałuża", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std74 = new(74, "Dina", "Czerwińska", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std75 = new(75, "Bertrand", "Sowa", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std76 = new(76, "Dorian", "Słowiński", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std77 = new(77, "Lilianna", "Balcerzak", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std78 = new(78, "Marceli", "Jagiełło", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std79 = new(79, "Hipolit", "Stolarczyk", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std80 = new(80, "Aurora", "Wiśniewska", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std81 = new(81, "Melania", "Sikorska", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std82 = new(82, "Emanuel", "Bąkowski", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std83 = new(83, "Estera", "Leszczyńska", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std84 = new(84, "Józef", "Mackiewicz", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std85 = new(85, "Bibiana", "Rucińska", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std86 = new(86, "Gertruda", "Bukowska", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std87 = new(87, "Rafał", "Kasprzyk", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std88 = new(88, "Ginewra", "Jagiełło", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std89 = new(89, "Bernard", "Korzeniowski", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std90 = new(90, "Borys", "Czapla", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std91 = new(91, "Anna", "Juszczak", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std92 = new(92, "Szczepan", "Nowiński", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std93 = new(93, "Luiza", "Szyszka", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std94 = new(94, "Lilla", "Żak", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std95 = new(95, "Nadzieja", "Pawlicka", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std96 = new(96, "Aaron", "Wojtczak", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std97 = new(97, "Hilary", "Olszewski", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std98 = new(98, "Penelopa", "Piechota", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std99 = new(99, "Leon", "Witczak", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std100 = new(100, "Roland", "Wilczek", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std101 = new(101, "Marianna", "Czajkowska", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std102 = new(102, "Wanda", "Czerniak", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std103 = new(103, "Grzegorz", "Adamski", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std104 = new(104, "Justyna", "Banach", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std105 = new(105, "Sebastian", "Michalski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std106 = new(106, "Salwator", "Bujak", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std107 = new(107, "Rozalia", "Młynarczyk", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std108 = new(108, "Alan", "Jurkowski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std109 = new(109, "Aleksy", "Michalik", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std110 = new(110, "Salwator", "Januszewski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std111 = new(111, "Leonard", "Serafin", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std112 = new(112, "Polikarp", "Witczak", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std113 = new(113, "Leon", "Krukowski", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std114 = new(114, "Władysław", "Zabłocki", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std115 = new(115, "Paula", "Tracz", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std116 = new(116, "Krystyna", "Chmiel", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std117 = new(117, "Adam", "Cybulski", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std118 = new(118, "Irena", "Stępniak", 18, false, random.Next(0, 99), random.Next(0, 99));
                Std std119 = new(119, "Eudoksja", "Szatkowska", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std120 = new(120, "Reginald", "Paluch", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std121 = new(121, "Irena", "Krzyżanowska", 20, false, random.Next(0, 99), random.Next(0, 99));
                Std std122 = new(122, "Karol", "Bartkowiak", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std123 = new(123, "Laurenty", "Winiarski", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std124 = new(124, "Berta", "Mika", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std125 = new(125, "Rebeka", "Kowalik", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std126 = new(126, "Seweryn", "Frąckowiak", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std127 = new(127, "Gabriel", "Koper", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std128 = new(128, "Porfiry", "Golec", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std129 = new(129, "Efrem", "Grzybowski", 18, true, random.Next(0, 99), random.Next(0, 99));
                Std std130 = new(130, "Bernard", "Jaśkiewicz", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std131 = new(131, "Amabela", "Nawrocka", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std132 = new(132, "Beniamin", "Szyszka", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std133 = new(133, "Michalina", "Wróblewska", 16, false, random.Next(0, 99), random.Next(0, 99));
                Std std134 = new(134, "Ernest", "Góral", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std135 = new(135, "Cecylia", "Tomala", 19, false, random.Next(0, 99), random.Next(0, 99));
                Std std136 = new(136, "Hanna", "Kopczyńska", 14, false, random.Next(0, 99), random.Next(0, 99));
                Std std137 = new(137, "Joachim", "Jackowski", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std138 = new(138, "Orestes", "Kaniewski", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std139 = new(139, "Dawid", "Wilk", 20, true, random.Next(0, 99), random.Next(0, 99));
                Std std140 = new(140, "Cecyliusz", "Bednarek", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std141 = new(141, "Karolina", "Gutowska", 17, false, random.Next(0, 99), random.Next(0, 99));
                Std std142 = new(142, "Salwator", "Dróżdż", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std143 = new(143, "Dorota", "Tomala", 15, false, random.Next(0, 99), random.Next(0, 99));
                Std std144 = new(144, "Natan", "Szulc", 17, true, random.Next(0, 99), random.Next(0, 99));
                Std std145 = new(145, "Seweryn", "Wąsik", 16, true, random.Next(0, 99), random.Next(0, 99));
                Std std146 = new(146, "Patryk", "Rusin", 19, true, random.Next(0, 99), random.Next(0, 99));
                Std std147 = new(147, "Maurycy", "Świątek", 14, true, random.Next(0, 99), random.Next(0, 99));
                Std std148 = new(148, "Ryszard", "Cieślik", 15, true, random.Next(0, 99), random.Next(0, 99));
                Std std149 = new(149, "Eudoksja", "Miller", 16, false, random.Next(0, 99), random.Next(0, 99));

                Cls cls0 = new(1001, "1A");
                Cls cls1 = new(1002, "1A");
                Cls cls2 = new(1003, "1A");
                Cls cls3 = new(1004, "2B");
                Cls cls4 = new(1005, "2B");
                Cls cls5 = new(1006, "3C");
                Cls cls6 = new(1007, "4D");

                Scl scl0 = new(2001, "ZSŁ Gdańsk");
                Scl scl1 = new(2002, "SP nr 11 Gdynia");
                Scl scl2 = new(2003, "SP nr 50 Gdańsk");

                cls0.AddStudent(std9);
                cls0.AddStudent(std15);
                cls0.AddStudent(std16);
                cls0.AddStudent(std22);
                cls0.AddStudent(std26);
                cls0.AddStudent(std29);
                cls0.AddStudent(std32);
                cls0.AddStudent(std34);
                cls0.AddStudent(std38);
                cls0.AddStudent(std43);
                cls0.AddStudent(std48);
                cls0.AddStudent(std50);
                cls0.AddStudent(std51);
                cls0.AddStudent(std55);
                cls0.AddStudent(std59);
                cls0.AddStudent(std62);
                cls0.AddStudent(std66);
                cls0.AddStudent(std71);
                cls0.AddStudent(std76);
                cls0.AddStudent(std77);
                cls0.AddStudent(std83);
                cls0.AddStudent(std87);
                cls0.AddStudent(std88);
                cls0.AddStudent(std92);
                cls0.AddStudent(std99);
                cls0.AddStudent(std103);
                cls0.AddStudent(std116);
                cls0.AddStudent(std120);
                cls0.AddStudent(std121);
                cls0.AddStudent(std125);
                cls0.AddStudent(std135);
                cls0.AddStudent(std143);

                cls1.AddStudent(std5);
                cls1.AddStudent(std6);
                cls1.AddStudent(std21);
                cls1.AddStudent(std27);
                cls1.AddStudent(std33);
                cls1.AddStudent(std52);
                cls1.AddStudent(std53);
                cls1.AddStudent(std68);
                cls1.AddStudent(std73);
                cls1.AddStudent(std74);
                cls1.AddStudent(std80);
                cls1.AddStudent(std90);
                cls1.AddStudent(std93);
                cls1.AddStudent(std109);
                cls1.AddStudent(std117);
                cls1.AddStudent(std118);
                cls1.AddStudent(std130);
                cls1.AddStudent(std134);
                cls1.AddStudent(std144);

                cls2.AddStudent(std10);
                cls2.AddStudent(std11);
                cls2.AddStudent(std14);
                cls2.AddStudent(std42);
                cls2.AddStudent(std47);
                cls2.AddStudent(std60);
                cls2.AddStudent(std70);
                cls2.AddStudent(std82);
                cls2.AddStudent(std84);
                cls2.AddStudent(std100);
                cls2.AddStudent(std102);
                cls2.AddStudent(std106);
                cls2.AddStudent(std107);
                cls2.AddStudent(std115);
                cls2.AddStudent(std119);
                cls2.AddStudent(std131);
                cls2.AddStudent(std138);
                cls2.AddStudent(std140);
                cls2.AddStudent(std142);
                cls2.AddStudent(std147);
                cls2.AddStudent(std149);

                cls3.AddStudent(std2);
                cls3.AddStudent(std4);
                cls3.AddStudent(std7);
                cls3.AddStudent(std12);
                cls3.AddStudent(std17);
                cls3.AddStudent(std23);
                cls3.AddStudent(std24);
                cls3.AddStudent(std37);
                cls3.AddStudent(std63);
                cls3.AddStudent(std64);
                cls3.AddStudent(std65);
                cls3.AddStudent(std95);
                cls3.AddStudent(std111);
                cls3.AddStudent(std124);

                cls4.AddStudent(std1);
                cls4.AddStudent(std8);
                cls4.AddStudent(std18);
                cls4.AddStudent(std25);
                cls4.AddStudent(std36);
                cls4.AddStudent(std39);
                cls4.AddStudent(std44);
                cls4.AddStudent(std56);
                cls4.AddStudent(std72);
                cls4.AddStudent(std75);
                cls4.AddStudent(std78);
                cls4.AddStudent(std79);
                cls4.AddStudent(std86);
                cls4.AddStudent(std97);
                cls4.AddStudent(std105);
                cls4.AddStudent(std108);
                cls4.AddStudent(std126);
                cls4.AddStudent(std127);
                cls4.AddStudent(std128);
                cls4.AddStudent(std132);

                cls5.AddStudent(std0);
                cls5.AddStudent(std19);
                cls5.AddStudent(std20);
                cls5.AddStudent(std28);
                cls5.AddStudent(std30);
                cls5.AddStudent(std31);
                cls5.AddStudent(std41);
                cls5.AddStudent(std45);
                cls5.AddStudent(std46);
                cls5.AddStudent(std49);
                cls5.AddStudent(std57);
                cls5.AddStudent(std61);
                cls5.AddStudent(std67);
                cls5.AddStudent(std69);
                cls5.AddStudent(std91);
                cls5.AddStudent(std94);
                cls5.AddStudent(std96);
                cls5.AddStudent(std98);
                cls5.AddStudent(std112);
                cls5.AddStudent(std113);
                cls5.AddStudent(std122);
                cls5.AddStudent(std123);
                cls5.AddStudent(std137);
                cls5.AddStudent(std145);

                cls6.AddStudent(std3);
                cls6.AddStudent(std13);
                cls6.AddStudent(std35);
                cls6.AddStudent(std40);
                cls6.AddStudent(std54);
                cls6.AddStudent(std58);
                cls6.AddStudent(std81);
                cls6.AddStudent(std85);
                cls6.AddStudent(std89);
                cls6.AddStudent(std101);
                cls6.AddStudent(std104);
                cls6.AddStudent(std110);
                cls6.AddStudent(std114);
                cls6.AddStudent(std129);
                cls6.AddStudent(std133);
                cls6.AddStudent(std136);
                cls6.AddStudent(std139);
                cls6.AddStudent(std141);
                cls6.AddStudent(std146);
                cls6.AddStudent(std148);


                scl0.AddClass(cls0);
                scl0.AddClass(cls3);

                scl1.AddClass(cls1);
                scl1.AddClass(cls4);
                scl1.AddClass(cls5);
                scl1.AddClass(cls6);

                scl2.AddClass(cls2);

                foreach (Scl scl in new Scl[] { scl0, scl1, scl2 })
                {
                    currentSchools.Add(scl);

                    Schools.Items.Add(scl.name);
                }
            }
            else
            {
                currentSchools = JsonSerializer.Deserialize<List<Scl>>(lines);

                foreach (Scl scl in currentSchools)
                {
                    Schools.Items.Add(scl.name);
                }
            }
        }
    }

    public class Scl
    {
        [JsonInclude]
        public int id;

        [JsonInclude]
        public string name;

        [JsonInclude]
        public List<Cls> classes;

        [JsonInclude]
        public int classIndex = 0; 

        public Scl(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.classes = new List<Cls>();
        }

        public void AddClass(Cls cls)
        {
            classes.Add(cls);
            classIndex++;
            cls.scl = this;
        }

        public int CountAllStudents()
        { 
            int count = 0;
            foreach(var cls in classes)
            {
                foreach (var std in cls.students)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public class Cls
    {
        [JsonInclude]
        public int id;

        [JsonInclude]
        public string name;

        [JsonInclude]
        public List<Std> students;

        [JsonInclude]
        public int studentIndex = 0;

        [JsonIgnore]
        public Scl scl;

        public Cls(int id, string name)
        {
            this.id = id;
            this.name = name;
            this.students = new List<Std>();  
        }

        public void AddStudent(Std std)
        {
            students.Add(std);
            std.cls = this;
            studentIndex++;
        }

        public (double manPercent, double womanPercent) CalculatePercents()
        {
            int manNumber = 0;
            int womanNumber = 0;

            double studentNumber = students.Count;

            foreach(var student in students)
            {
                if (student.sex == true)
                {
                    manNumber++;
                }
                else
                {
                    womanNumber++;
                }
            }

            double manPercent = manNumber / studentNumber * 100;
            double womanPercent = womanNumber / studentNumber * 100;

            return (Math.Round(manPercent, 2), Math.Round(womanPercent, 2));
        }

        public double AverageInteligence()
        {
            double Inteligence = 0;

            foreach (var student in students)
            {
                Inteligence += student.inteligence;
            }

            Inteligence /= students.Count;

            return Math.Round(Inteligence, 2);
        }

        public double AverageStrenght()
        {
            double Strenght = 0;

            foreach (var student in students)
            {
                Strenght += student.strenght;
            }

            Strenght /= students.Count;

            return Math.Round(Strenght, 2);
        }
    }

    public class Std
    {
        [JsonInclude]
        public int id;

        [JsonInclude]
        public string name;

        [JsonInclude]
        public string surname;

        [JsonInclude]
        public int age;

        [JsonInclude]
        public bool sex;

        [JsonInclude]
        public int inteligence;

        [JsonInclude]
        public int strenght;

        [JsonIgnore]
        public Cls cls;

        public Std(int id, string name, string surname, int age, bool sex, int inteligence, int strenght)
        {
            this.id = id;
            this.name = name;
            this.surname = surname;
            this.age = age;
            this.sex = sex;
            this.inteligence = inteligence;
            this.strenght = strenght;
        }
    }
}