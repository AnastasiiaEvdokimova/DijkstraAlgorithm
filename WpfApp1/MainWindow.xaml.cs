using System;
using System.Collections;
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

namespace WpfApp1
{

    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private class Item
        {
            public string Name;
            public int Value;
            public Item(string name, int value)
            {
                Name = name; Value = value;
            }
            public override string ToString()
            {
                
                return Name;
            }
        }
        static bool flag = false;

            static void InputMatrixSetup(Grid InputMatrix, int size) {
            InputMatrix.Children.Clear();
            InputMatrix.ColumnDefinitions.Clear();
            InputMatrix.RowDefinitions.Clear();
            for (int i = 0; i < size; i++)
            {
                InputMatrix.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star), });
                InputMatrix.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star), });
                for (int j = 0; j < size; j++)
                {
                  
                    TextBox t = new TextBox();
                    t.Width = 35;
                    if (j == i)
                    {
                        t.Text = "0";
                    }
                 
                    t.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    t.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    t.SetValue(Grid.RowProperty, j);
                    t.SetValue(Grid.ColumnProperty, i);
                    t.TextAlignment = TextAlignment.Center;
                    //  t.Text = "";
                    InputMatrix.Children.Add(t);

                }

            }
        
              
            
        }

 
        private void MatrixSize_TouchEnter(object sender, TouchEventArgs e)
        {
            MatrixSize.Text = "";
        }

        private void MatrixSize_TextChanged(object sender, TextChangedEventArgs e)
        {
         
        }

        private void MatrixSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            InputMatrix.Children.Clear();
            InputMatrix.ColumnDefinitions.Clear();
            InputMatrix.RowDefinitions.Clear();
            int size = 0;
            size = MatrixSize.SelectedIndex;

            if (size > 0)
            {
            if (!flag)
                {
                    flag = true;
                    MatrixSize_SelectionChanged(sender, e);
                }
                StartVertex.Items.Clear();
                FinalVertex.Items.Clear();
                for (int i = 1; i <= size; i++)
                {
                    StartVertex.Items.Add(new Item(Convert.ToString(i), i));
                    FinalVertex.Items.Add(new Item(Convert.ToString(i), i));
                }
            
                InputMatrixSetup(InputMatrix, size);
            }
         
        }

        private void FindSolution_Click(object sender, RoutedEventArgs e)
        {
            int stopcounter = 0;
            int columns = InputMatrix.ColumnDefinitions.Count;
            int rows = InputMatrix.RowDefinitions.Count;
            double[,] Edges = new double[columns, rows];
            //Запись матрицы смежности 
            for (int c = 0; c < InputMatrix.Children.Count; c++)
            {
                TextBox t = (TextBox)InputMatrix.Children[c];
                int row = Grid.GetRow(t);
                int column = Grid.GetColumn(t);
                //Проверка на корректность введенных данных
                if (!double.TryParse(t.Text, out Edges[column, row]))
                {
                    MessageBox.Show("Проверьте коректность введенных данных");
                    return;
                }

            }
         //Проверка матрицы смежности
                for (int i = 0; i < columns; i++)
                    for (int j = 0; j < rows; j++)
                    
                        if (Edges[i, j] != Edges[j, i])
                        {
                            MessageBox.Show("Проверьте коректность введенных данных. Матрица смежности должна быть зеркальной относительно главной диагонали");        
                                return;
                        }
                else if ((i == j) && (Edges[i, j] != 0))
                    {
                        MessageBox.Show("Проверьте коректность введенных данных. Матрица смежности не должна иметь петлей");
                        return;
                    }
               if ((StartVertex.SelectedIndex == -1)||(FinalVertex.SelectedIndex == -1))
            {
                MessageBox.Show("Не введено начало и конец пути");
                return;
            }
            //Объявление массива меток
            double[] MarksArray = new double[columns];
            bool[] MarksArrayFinal = new bool[columns];
           
            bool finita = false;
            int StarterVertex = Convert.ToInt32(StartVertex.SelectedItem.ToString())-1;
            int CurrentVertexIndex = StarterVertex; //Начинаем искать со стартовой вершины
            for (int i = 0; i<rows; i++)
            {
                MarksArray[i] = -1;
                MarksArrayFinal[i] = false;
            }
            //Ставим постоянную метку на стартовую вершину
            MarksArray[StarterVertex] = 0;
            MarksArrayFinal[StarterVertex] = true;
            do
            {
                //Рассматриваем все пути из текущей вершины
                for (int i = 0; i < rows; i++)
                {
                    if ((Edges[CurrentVertexIndex, i] != 0)) // есть путь в вершину из текущей рассматриваемой
                        if ((MarksArray[i] > Edges[CurrentVertexIndex, i] + MarksArray[CurrentVertexIndex]) || (MarksArray[i] == -1)) //метка меньше или не записана
                        {
                            MarksArray[i] = Edges[CurrentVertexIndex, i] + MarksArray[CurrentVertexIndex]; //метка = путь из рассматриваемой вершины + метка этой вершины
                        }

                }
                //ищем опорную нерассмотренную вершину для сравнения меток
                for (int i = 0; i < rows; i++)
                {
                    if ((MarksArray[i] != -1)&&(!MarksArrayFinal[i])) //Если временная метка определена
                    {
                        CurrentVertexIndex = i;
                        break;
                    }
                   
                }
                //Ищем минимальную среди существующих меток
                for (int i = 0; i < rows; i++)
                {
                    if ((MarksArray[i] != -1) && (MarksArray[i]<MarksArray[CurrentVertexIndex]) && (!MarksArrayFinal[i]))
                    {
                        CurrentVertexIndex = i;
                    }
                }
                MarksArrayFinal[CurrentVertexIndex] = true; //помечаем как постоянную
                if (CurrentVertexIndex == Convert.ToInt32(FinalVertex.SelectedItem.ToString()) -1) //Если дошли до конца, завершаем работу цикла
                    finita = true;
                stopcounter++; //увеличиваем счетчик цикла
                //Проверяем на отсутствие зацикливания программы
                if (stopcounter > rows+1) //Цикл не должен работать больше раз, чем есть вершин
                {
                    MessageBox.Show("Между выбранными вершинами пути не существует");
                    return;
                }
            } while (!finita);
            //Выводим минимальное расстояние
            SolutionDisplay.Text = "Минимальное расстояние = " + Convert.ToString(MarksArray[CurrentVertexIndex] + "\n");
            //Строим кратчайший путь
            StringBuilder finalAnswer = new StringBuilder();
            do
            {             
                    finalAnswer.Insert(0, " - " + Convert.ToString(CurrentVertexIndex + 1));
                for (int i = 0; i < rows; i++)
                {
                    if (i!=CurrentVertexIndex)
                    if ((MarksArray[CurrentVertexIndex] == MarksArray[i] + Edges[CurrentVertexIndex, i])&&(Edges[CurrentVertexIndex, i]!=0) && (MarksArrayFinal[i] == true))
                    {
                        CurrentVertexIndex = i;
                            break;
                    }
                }

            }
            while (CurrentVertexIndex != StarterVertex);
            //Добавляем стартовую вершину
            finalAnswer.Insert(0, Convert.ToString(StarterVertex + 1));
            //Выводим ответ
      SolutionDisplay.Text += "Требуется проделать следующий путь: " + finalAnswer.ToString();
        }
    }
}
