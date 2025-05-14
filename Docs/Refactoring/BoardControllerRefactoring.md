# Board Controller Refactoring
## 목표
BoardController 클래스를 SOLID 원칙에 따라 리팩토링합니다.

## 결과
### 다이어그램
![diagram](https://github.com/chahoseong/Employment-Task/blob/main/Docs/Refactoring/BoardController%20Refactoring.png?raw=true)
### 클래스
#### Builder
`BoardController`가 `StageData`로 레벨을 생성하는 일을 Builder 클래스로 분할했습니다. Builder의 종류는 다음과 같습니다.
- Wall
- BoardBlock
- Block
- Quad
- Stage
`StageBuilder`가 최종적으로 레벨을 구성하고 그에 대한 정보는 `StageContext` 객체로 반환합니다.

#### GameManager
`BoardController`와 `Builder`를 중재하기 위해 `GameManager` 클래스를 추가했습니다. 그리고 게임에서 전반적으로 사용할 **Service** 객체들을 초기화하고 `ServiceLocator`에 주입시켜주는 일도 맡습니다.

#### BoardController
BoardController는 이제 특정 BoardBlockObject에 올라가 있는 BlockGroup이 제거될 수 있는지 검사하는 일만 수행합니다. 제 생각에는 현재 BoardController와 BoardBlockObject가 서로 직접적으로 알고 부분을 인터페이스로 구성하여 의존성을 줄이면 좋겠다고 생각합니다만 시간이 부족하여 진행하지 못했습니다.

#### Service
블록이 파괴될 때, 파티클 이펙트를 생성한다던가, Material을 교체하는 등, 전역적으로 수행해야할 작업들은 Service로 구성하였습니다. `ServiceLocator`를 통해서 바로 접근해서 사용할 수 있습니다.
