using System;

namespace ExchangeModel.Auxiliary
{
    public static class XArrayExtension
    {
        /// <summary>
        /// Получение под массива из массива
        /// </summary>
        /// <typeparam name="T">результирующий под массив</typeparam>
        /// <param name="array">исходный массив</param>
        /// <param name="start">с какого элемента</param>
        /// <param name="count">какое количество элементов</param>
        /// <returns></returns>
        public static T[] SubArray<T>(this T[] array, int start, int count)
        {
            T[] result = new T[count];
            Array.Copy(array, start, result, 0, count);
            return result;
        }
    }
}
