using System.Text;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

Console.WriteLine("Средний балл состоит из оценок за уроки, контрольную и проект");

Console.Write("Введите оценку за уроки: ");                                
uint.TryParse(Console.ReadLine(), out uint taskScore);

Console.Write("Введите оценку за контрольную: ");                                 
uint.TryParse(Console.ReadLine(), out uint testScore);

Console.Write("Введите оценку за проект: ");                                     
uint.TryParse(Console.ReadLine(), out uint projectScore);

// Math.Round с округлением до 1 знака для удобного отображения
double averageScore = Math.Round(((taskScore + testScore + projectScore) / 3.0), 1);
                                                                                 
Console.Clear();

Console.WriteLine("Итоговая оценка состоит из среднего балла + бонусные баллы - штрафные баллы");

Console.Write("Введите штрафные баллы: ");
uint.TryParse(Console.ReadLine(), out uint penalty);

Console.Write("Введите бонусные баллы: ");                                        
uint.TryParse(Console.ReadLine(), out uint bonus);                              

// Convert использован осмысленно. Ради математичкого округление не стал переусложнять структуру неизучеными методами
int finalScore = Convert.ToInt32(averageScore + bonus - penalty);

Console.Clear();

Console.WriteLine($"Средний балл: {averageScore}");                                      
Console.WriteLine($"Итоговая оценка: {finalScore}");                        
                                                         
Console.ReadKey();

