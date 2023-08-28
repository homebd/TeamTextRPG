# TeamTextRPG

### UI 설명

1. 왼쪽 위 여백: 씬을 시각적으로 표현하는 영역
2. 왼쪽 아래 상자: 옵션을 보여주고 입력 받는 영역 (Option & Input)
3. 오른쪽 상자: 행동의 결과나 오류 등을 보여주는 영역 (Log)

### 필수 요구 사항

1. 게임 시작 화면 - '마을'로 두고, 저장된 데이터를 불러오거나 생성하는 씬 구성
2. 상태보기 - 장착 중인 장비와 스탯 및 돈과 경험치 표기, UI 상으로 한 칸 남음(펫이나 유물, 증표 구상)
3. 인벤토리 - 카테고리로 나누어 구현

### 선택 요구 사항

1. 아이템 정보를 Item 클래스로 정의
2. 아이템 정보를 DataManager의 _items 배열로 관리
3. 아이템 추가하기 - 파츠를 나눠 파츠 별로 총 10개씩 추가
4. 콘솔 꾸미기 -  각 씬의 타이틀과 던전 잠금에만 색상 사용
5. 인벤토리 크기 맞춤 - 한글이 맞추기 까다로워 Console.SetCursorPosition을 통해 맞춤
6. 인벤토리 정렬하기 - 카테고리로 나누고 추가로 정렬 메뉴도 구현
7. 상점 - 구매(구매 시 상점에서 해당 아이템 삭제) 및 판매(85% 가격, 판매 시 해당 아이템 상점에 추가) 구현
8. 장착 개선 - 무기, 방어구(투구, 갑옷, 바지, 신발) 총 5종류로 나눠 구현
9. 던전 입장 - [, ]키를 눌러 페이지 넘기기 및 10개의 스테이지 구현, 스테이지에서 아이템 드랍 구현
10. 휴식 기능 - 후반에 체력이 커지면 번거로워서 3종류로 구현, 진짜 쉬는 것처럼 딜레이 추가
11. 레벨업 기능 - 던전 난이도에 따라 경험치 차등 지급하도록 구현, '상태 보기'에서 레벨 옆에 시각적으로 구현
12. 게임 저장하기 - 최초에 게임 접속 시 파일 생성, 불러오기 구현 (ID가 파일 이름: "ID.json")
13. 나만의 기능 - 강화, 아이템 드랍, 던전 해금
