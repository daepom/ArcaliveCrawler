# ArcaliveCrawler / 아카라이브 크롤러

***
**아카라이브 API 관련 변경 사항으로 인해 더 이상 작동하지 않습니다!**
***

![ex](preview.png)\
커뮤니티 사이트 [아카라이브](https://arca.live)용 크롤러 프로그램이자 라이브러리. 갤창랭킹을 집계하기 위해 개발했습니다.\
이 프로그램의 사용으로 생기는 문제는 사용자에게 있습니다.\
코드가 매우 난잡해서 점차 정돈해 나갈 예정..

***

**일반 사용자용 설명서**

프로그램을 다운받고 실행합니다. 여기에서 [HERE!](https://github.com/tjgus1668/ArcaliveCrawler/releases)

갤창랭킹을 출력하는 등의 작업을 하기 전에, 크롤링이란 작업이 필요합니다.
* 크롤링 데이터 파일 출력 방법
  * 채널의 url을 입력합니다. https://arca.live/b/xxx 라는 주소를 가진 채널의 경우 xxx를 입력하면 됩니다.
  * 크롤링할 시간대를 설정하고, 크롤링! 버튼을 클릭합니다.
  * 크롤링에는 오랜 시간이 소요됩니다. (한 달치의 경우 1~4시간 정도)
  * 크롤링이 완료되면 데이터 파일을 원하는 곳에 저장합니다.
* 크롤링 데이터 파일 사용 방법
  * 일반적인 상황이라면 통계 출력 외에는 사용할 일이 없습니다. 통계 출력을 클릭합니다.
  * 파일 선택을 통해 아까 저장한 데이터 파일을 불러오고, 아래서 원하는 기능을 고릅니다.
  * 가공된 파일을 원하는 곳에 저장합니다.
  
***

**개발자용 설명서**

프로그램에 있는 Arcalive.dll을 작업하는 프로젝트에 참조로 추가합니다.\
생성자 사용 예시:

    ArcaliveCrawler ac = new ArcaliveCrawler("yuzusoft");
    
검색 기능을 이용해 채널 이름을 구할 수도 있습니다.

    string str = ArcaliveCrawler.GetChannelLink("유즈소프트");
    ArcaliveCrawler ac = new ArcaliveCrawler(str);
    
게시판 크롤링 예시:

    List<string> posts = ac.CrawlBoards(startDateTime, endDateTime);

댓글 작성자, 글 내용, 댓글 내용 등의 세부적인 정보를 얻기 위해서는 다른 함수를 추가로 사용해야 합니다.\
글 크롤링 예시:

    posts = ac.CrawlPosts(posts);
    
크롤링한 결과를 직렬화 하는 예시:

    ArcaliveCrawler.SerializePosts(posts, filename);
    
저장된 .dat 파일을 역직렬화 하는 예시:

    var posts = ArcaliveCrawler.DeserializePosts(fileName);

