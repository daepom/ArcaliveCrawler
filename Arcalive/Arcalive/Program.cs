using System;

namespace Arcalive
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("기능을 선택해주세요.\n" +
                    "1. 채널 링크 출력\n" +
                    "2. 갤창 리포트 출력\n" +
                    "3. 글 데이터 파일 생성\n" +
                    "종료: Ctrl + C");
                int a = int.Parse(Console.ReadLine());
                switch (a)
                {
                    case 1:
                        Console.WriteLine("채널 검색 키워드를 입력해주세요.");
                        break;

                    case 2:

                        break;

                    default:
                        break;
                }
                Console.WriteLine("아무키나 누르세요..");
                Console.ReadKey();
            }
        }
    }
}