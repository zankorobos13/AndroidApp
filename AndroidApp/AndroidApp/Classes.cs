using System;
using System.Collections.Generic;
using System.Text;
using MySqlConnector;
using System.Security.Cryptography;

namespace AndroidApp
{
    
    public class Transaction
    {
        public static Transaction[] transactions = new Transaction[0];
        public static Transaction current_transaction = null;
        private static Transaction[] transactions_memory = new Transaction[0];
        public static string[] used_categories = new string[0];

        public static readonly string default_categories = "Транспорт;Аптеки;Дом;Одежда;Бизнес;Зарплата;Супермаркеты;Такси;Прочее";
        public static string sort_method = "dateTime_newest";

        public readonly decimal sum;
        public readonly string category;
        public string comment;
        public readonly DateTime dateTime;

        /*public decimal sum { get; set; }
        public string category { get; set; }
        public string comment { get; set; }
        public DateTime dateTime { get; set; }*/


        public Transaction(decimal sum, string category, string comment, DateTime dateTime)
        {
            this.sum = sum;
            this.category = category;
            this.comment = comment;
            this.dateTime = dateTime;
        }

        public static void RememberTransactions()
        {
            transactions_memory = transactions;
        }

        public static void UploadTransactionsFromMemory()
        {
            transactions = transactions_memory;
        }

       public static void SortByIncomeExpense(bool isIncome = true)
        {
            UploadTransactionsFromMemory();
            Transaction[] sorted_transactions = new Transaction[0];

            for (int i = 0; i < transactions.Length; i++)
            {
                if (isIncome && transactions[i].sum >= 0)
                    Append(ref sorted_transactions, transactions[i]);
                else if (!isIncome && transactions[i].sum < 0)
                    Append(ref sorted_transactions, transactions[i]);
            }

            transactions = sorted_transactions;
        } 


        public static void SortByCategory(string category)
        {
            UploadTransactionsFromMemory();
            Transaction[] sorted_transactions = new Transaction[0];

            for (int i = 0; i < transactions.Length; i++)
            {
                if (transactions[i].category == category)
                    Append(ref sorted_transactions, transactions[i]);
            }

            transactions = sorted_transactions;
        }

        public static void SortByDateTime(bool increasing = true)
        {
            UploadTransactionsFromMemory();

            for (int i = 0; i < transactions.Length - 1; i ++)
            {
                for (int j = 0; j < transactions.Length - 1; j++)
                {
                    
                    if (DateTime.Compare(transactions[j].dateTime, transactions[j + 1].dateTime) < 0)
                    {
                        Transaction temp = transactions[j];
                        transactions[j] = transactions[j + 1];
                        transactions[j + 1] = temp;
                    } 
                }
            }

            if (!increasing)
                Array.Reverse(transactions);
        }

        private static void CutArr(ref Transaction[] arr, int x)
        {
            Transaction[] new_arr = new Transaction[x];

            for (int i = 0; i < new_arr.Length; i++)
                new_arr[i] = arr[i];

            arr = new_arr;
        }

        public static void SortByTimeInterval(TimeInterval interval)
        {
            UploadTransactionsFromMemory();
            Transaction[] sorted_transactions = new Transaction[transactions.Length];
            int j = 0;

            switch (interval)
            {
                case TimeInterval.Day:
                    for (int i = 0; i < transactions.Length; i++)
                    {
                        if (transactions[i].dateTime.Year == DateTime.Now.Year && transactions[i].dateTime.Month == DateTime.Now.Month && transactions[i].dateTime.Day == DateTime.Now.Day)
                            sorted_transactions[j++] = transactions[i];
                    }
                    break;
                case TimeInterval.Week:
                    for (int i = 0; i < transactions.Length; i++)
                    {
                        if (transactions[i].dateTime > DateTime.Now.AddDays(-7))
                            sorted_transactions[j++] = transactions[i];
                    }
                    break;
                case TimeInterval.Month:
                    for (int i = 0; i < transactions.Length; i++)
                    {
                        if (transactions[i].dateTime.Year == DateTime.Now.Year && transactions[i].dateTime.Month == DateTime.Now.Month)
                            sorted_transactions[j++] = transactions[i];
                    }
                    break;
                case TimeInterval.ThreeMonths:
                    for (int i = 0; i < transactions.Length; i++)
                    {
                        if (transactions[i].dateTime > DateTime.Now.AddMonths(-3))
                            sorted_transactions[j++] = transactions[i];
                    }
                    break;
                case TimeInterval.Year:
                    for (int i = 0; i < transactions.Length; i++)
                    {
                        if (transactions[i].dateTime.Year == DateTime.Now.Year)
                            sorted_transactions[j++] = transactions[i];
                    }
                    break;
            }

            CutArr(ref sorted_transactions, j);
            transactions = sorted_transactions;
        }


        public static void AddTransaction(Transaction transaction)
        {
            Append(ref transactions, transaction);
            Append(ref transactions_memory, transaction);

            bool isCategoryAlredyUsed = false;
            for (int i = 0; i < used_categories.Length; i++)
            {
                if (used_categories[i] == transaction.category)
                {
                    isCategoryAlredyUsed = true;
                    break;
                }
            }
            if (!isCategoryAlredyUsed)
                Append(ref used_categories, transaction.category);
        }

        public static void ClearTransaction()
        {
            transactions = new Transaction[0];
            transactions_memory = new Transaction[0];
            used_categories = new string[0];
            sort_method = "dateTime_newest";
        }

        private static void Append(ref Transaction[] arr, Transaction transaction)
        {
            Transaction[] new_arr = new Transaction[arr.Length + 1];
            new_arr[0] = transaction;

            if (transaction.comment == null || transaction.comment.Trim() == string.Empty)
                transaction.comment = "Вы не оставили комментарий к транзакции";

            for (int i = 1; i < new_arr.Length; i++)
                new_arr[i] = arr[i - 1];
                
            arr = new_arr;
        }

        private static void Append(ref string[] arr, string category)
        {
            string[] new_arr = new string[arr.Length + 1];
            new_arr[0] = category;

            for (int i = 1; i < new_arr.Length; i++)
                new_arr[i] = arr[i - 1];

            arr = new_arr;
        }

    }

    public enum TimeInterval
    {
        Day,
        Week,
        Month,
        ThreeMonths,
        Year
    }



    public class DB
    {
        readonly MySqlConnection connection = new MySqlConnection("server=remotemysql.com;port=3306;username=NqaN7lnsn5;password=WGxtoGYopq;database=NqaN7lnsn5");

        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }

        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }

    public static class Encrypt
    {
        public static string Sha256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
    
    

}
