// See https://aka.ms/new-console-template for more information
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Engine.OrLog.Models.Hand;
/*
int a = 456;
int b = 7;
int c = 66;
var type = c >> a & b; // у операции >> приоритет выше чем у &
Console.WriteLine($"{nameof(a)} = {a}, in binary is : {Convert.ToString(a, toBase: 2)}");
Console.WriteLine($"{nameof(b)} = {b}, in binary is : {Convert.ToString(b, toBase: 2)}");
Console.WriteLine($"{nameof(c)} = {c}, in binary is : {Convert.ToString(c, toBase: 2)}");
Console.WriteLine($"{nameof(a)} & {nameof(b)}, in binary is : {Convert.ToString(a & b, toBase: 2)}");
Console.WriteLine($"{nameof(a)} & {nameof(b)}, in deciaml is : {Convert.ToString(a & b, toBase: 10)}");
Console.WriteLine($"{nameof(type)} = {nameof(c)} >> {nameof(a)} & {nameof(b)}, is equal: {Convert.ToString(type, toBase: 2)}");
*/
//Console.WriteLine("dfd");



namespace HandDataCollector
{
    static class Program {
        static void Main()
        {           
                //тестовая программа, коорая считывает лог, записанный снифером
                FileReader.ReadFileToMessageStack();
                Thread t = new Thread(() => MessageDispatcher.RunDebugTest(false));
                t.Start();
                Console.WriteLine("Program Main is here");
                
        }
    }



}






