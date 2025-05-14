# BlockDragHandler Refactoring
## 목표
BlockDragHandler 클래스를 SOLID 원칙에 맞게 리팩토링을 합니다.

## 결과
### 다이어그램
![diagram](https://github.com/chahoseong/Employment-Task/blob/main/Docs/Refactoring/BlockDragHandler%20Refactoring.png)
### 클래스
#### BlockDragHandler
입력 장치의 입력을 받아서 전달하는 일을 합니다.

#### BlockGroup
`IDraggable` 인터페이스를 구현하여, BlockDragHandler의 입력을 받아서 그에 맞는 동작을 수행합니다.
`BlockGroup`은 `BlockPiece` 객체로 구성되며, 메시지를 중재하는 역할을 합니다. 구체적인 동작은 `BlockPiece`, `BlockMovement`, `BlockCollision` 객체가 처리합니다.

#### BlockPiece
블록을 구성하는 최소단위입니다.

#### BoardBlockObject
보드를 구성하는 최소 단위입니다. `BlockGroup`이 특정 `BoardBlockObject`에 위치하면 `BoardBlockObject`는 해당 블록을 처리할 수 있다면 처리합니다.
이 부분은 더 리팩토링을 해야된다고 생각하지만 시간이 부족하여 리팩토링을 하지 못하였습니다.
