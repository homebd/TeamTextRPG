# TeamTextRPG

# 팀 노션 페이지
https://www.notion.so/09-c1b84370f617461d968c9c84ace565d7

### UI 설명

1. 왼쪽 위 여백: 씬을 시각적으로 표현하는 영역
2. 왼쪽 아래 상자: 옵션을 보여주고 입력 받는 영역 (Option & Input)
3. 오른쪽 상자: 행동의 결과나 오류 등을 보여주는 영역 (Log)

### 초기 화면 

아스키 아트를 시도해 보았지만 거창한 그림은 표현하지 못하였고 Text RPG라는 글을 표현하는데 그쳤습니다.

### 회원 가입 (1번 입력)

각각 아이디(파일명), 닉네임(게임 내의 이름), 직업 선택 3개의 과정으로 이루어져 있습니다.
직업은 전사, 마법사, 궁수로 이루어져 있습니다.
직업의 종류에 따라 각각의 초기 능력치 및 성장 능력치, 스킬들이 Save파일을 만들때 모두 입력되게 제작되었습니다.
초기 회원 가입시 자동으로 로그인 되어 마을Scene으로 넘어가게 됩니다.
	
### 로그인 (2번 입력)

로그인 방법은 아이디를 입력하는 1가지의 과정으로 진행됩니다.
기존의 Save폴더 내부에 입력한 아이디와 같은 이름의 Json파일이 존재한다면 그 파일을 Load하는 방식으로 진행됩니다.

### 게임 종료 (0번 입력)

그 즉시 게임을 종료합니다.

### 상태창
* 상태 창은 마을에서 1번을 입력해 진입할 수 있다.
* 상태 창에서는 플레이어의 현재 스테이터스를 확인할 수 있다. 스탯 옆에 (+) 등으로 표기되는 수치는 장비아이템 등으로 얻어지는 수치이다.
* 상태 창에서 1번을 입력해 플레이어의 닉네임을 변경할 수 있다.
* 상태 창에서 2번을 입력해 가지고 있는 스킬 목록을 확인하고 관리할 수 있다.

### 인벤토리
* 인벤토리 창은 마을에서 2번을 입력해 진입할 수 있다.
* 인벤토리 창에서는 가지고 있는 아이템 목록을 카테고리별로 정렬해서 볼 수 있으며 아이템 스탯과 설명도 표시된다.
* 인벤토리 창에서 1번을 입력해 가지고 있는 장비 아이템을 장착할 수 있다.

### 상점
* 상점 창은 마을에서 3번을 입력해 진입할 수 있다.
* 상점 창에서는 아이템을 카테고리별로 정렬해서 볼 수 있으며 아이템 스탯과 가격, 소지금을 확인할 수 있다.
* 상점에 있는 아이템은 기본적인 상점 아이템에 플레이어가 판매한 아이템을 가지고 있으며 장비 아이템의 경우 구매하면 상점 목록에서 사라진다.
* 아이템 판매를 선택하면 판매가격이 표시되며 판매가는 구매가의 30%이다.

### 던전
* 던전 입장 창은 마을에서 4번을 입력해 진입할 수 있다.
* 던전 입장 창에서는 현재 레벨과 경험치, 방어력을 확인할 수 있다.
* 던전 입장 화면에서는 3개의 던전이 보여지며 '[' , ']' 키를 입력해 페이지를 이동할 수 있고 '1', '2', '3' 키를 입력해 입장할 수 있다.
* 던전은 총 10단계가 존재하며 이전 단계의 던전을 클리어해야 입장가능하다.
* 각 던전마다 권장 방어력과 보상이 있으며 보상은 골드 보상과 장비 보상이 있다.
장비 보상은 해당 던전에 맞는 수준의 아이템 세트이며 드랍 확률은 각 파츠당 10%다.

### 휴식
* 휴식 창은 마을에서 5번을 입력해 진입할 수 있다.
* 휴식 창에서는 현재 체력과 마나를 막대 바를 통해 알 수 있고 소지금도 표시된다.
* 휴식의 종류는 3가지로 회복량 당 가격은 동일하고 회복 수치만 달라진다.

### 강화
* 강화 창은 마을에서 6번을 입력해 진입할 수 있다.
* 강화 창에서는 가지고 있는 장비 아이템 목록과 강화비용, 강화확률, 소지금을 확인할 수 있다.
* 강화 확률은 100% -> 75% -> 37.5%와 같이 줄어든다.
* 강화한 아이템은 강화 수치 * 5를 1.5승한 퍼센트만큼 스탯이 증가한다.
* 아이템은 강화 중에 실패하게 되면 실패 확률의 절반의 확률만큼으로 파괴될 수 있다.

### 전투

* 전투에 들어가면 체력과 마나를 UI로 보여주고, 해당 던전에 등록된 몬스터를 중복 조합으로 1~4마리 뽑아 카드 형식으로 보여준다.
* '1. 기본 공격, 2. 특수 공격, 3. 소모품, 0. 도망치기' 4가지의 배틀 커맨드가 존재한다.
* 기본 공격 시, 몬스터를 선택해 공격하거나 0을 눌러 배틀 커맨드를 다시 선택할 수 있다.
* 특수 공격 시, 스킬을 고른 뒤 타겟을 골라 사용하거나, 0을 눌러 배틀 커맨드를 다시 선택할 수 있다.
* 소모품 시, 아이템을 골라 플레이어에게 사용하거나, 수류탄의 경우 적에게 피해를 줄 수 있다.

### 스킬

* 스킬은 '마을 - 1. 상태 보기 - 2. 스킬 관리' / '마을 - 4. 던전 입장 - 1~3. 던전 - 2. 특수 공격'에서 확인할 수 있다.
* 스킬은 회원가입 시 선택한 직업에 따라 분배되며, 해금 시스템 없이 마나에 맞게 사용할 수 있다.
  - 마나란, 스킬을 사용할 때 드는 비용으로 레벨업을 통해서만 최대 마나를 올릴 수 있으며
  - 상점 내 소모품란 마나 물약을 구매 후 사용해 회복하거나, '마을 - 5. 휴식하기'에서 비용을 지불해 회복할 수 있다. 레벨업을 통해서도 회복 가능하다.
* 스킬 관리에서 스킬의 순서를 바꿀 수 있다.
* 스킬의 종류에는 직접 데미지형, 지속 데미지형, 버프형, 디버프형이 있고 해당 스킬이 광역기인지 아닌지로 나뉜다.

### 아이템 
* 분류기능, 전체, 무기, 투구, 갑옷, 바지, 신발, 소모 총 7가지.
* 아이템 사용, 각각 '장착'과 '사용'을 위해 다른 메서드를 만들지 않고 둘 다 Wear 메서드를 사용. 
* Wear 메서드에서 아이템 타입에 따라 다르게 동작. 소모품 일 경우 'ItemUse' 메서드를 호출. 아이템 갯수를 줄이고 갯수가 0이면 인벤토리에서 삭제
* ItemUse에서는 다시  Enum, UsableItemTypes을 보고 아이템의 효과를 구현한다.
* 전투에서 '소모'로 분류되는 아이템만 보여줌.
* 사용 시 Wear로 호출되는 건 같지만 공격 아이템의 경우 BattleManager의 메서드를 사용.
* 아이템 id에 따라 스킬을 생성하여 Battle메서드에 넣어줌으로서 발동
* 상점에서 구매 시 인벤토리에 똑같은 아이템이 있는 지 확인하고 다르게 동작. 인벤토리에 추가 혹은 아이템의 갯수 증가

### Managers

##### GamaManager

* 싱글톤으로 유지되어 서로 간의 참조를 도와주는 매니저입니다.

##### DataManager

* 세이브, 로드부터 플레이어, 몬스터풀 등의 전반적인 모든 데이터를 관리하는 매니저입니다.

##### SceneManager

* 현재 몇 번째 씬인지를 저장하고 해당 인덱스에 맞게 씬을 보여주는 매니저입니다.
* 각 씬에서 일어나는 일을 관리합니다.

##### BattleManager

* 전투를 관리하는 매니저입니다.
* 플레이어나 몬스터가 사용한 공격 및 스킬을 리스트에 등록하여 턴에 따라 스킬을 사용해줍니다.

##### UIManager

* 콘솔에 UI Container를 만드는 등의 기능을 관리하는 매니저입니다.
