# 자율운항 보트 시뮬레이션 (6조)

2026년 1학기 수업 팀 프로젝트 과제입니다.
Unity에서 보트를 직접 조종하거나 자율운항 모드로 전환해, 장애물을 피하면서 제한 시간 안에 목적지까지 가는 시뮬레이션입니다.

## 출처

Unity Technologies의 데모 프로젝트 [Boat Attack](https://github.com/Unity-Technologies/BoatAttack)을 바탕으로 만들었습니다.
섬 맵, 물·부력 시스템, 보트 모델, 기존 AI/카메라 스크립트는 원본 그대로이고, 원본 라이선스는 `come/LICENSE.md`(Unity Companion License)에 있습니다.

팀에서 새로 작성한 스크립트는 아래와 같습니다.

| 파일 | 내용 |
|---|---|
| `Assets/Scripts/BoatAutoNav.cs` | 자율운항. 웨이포인트를 순서대로 지난 뒤 목적지로 이동하고, 앞쪽 9방향 레이캐스트로 장애물을 감지해 피함 |
| `Assets/Scripts/Simulation_Manager.cs` | 타이틀 → 시작 → 제한 시간(기본 60초) → 성공/실패 화면 흐름 관리 |
| `Assets/Scripts/BoatHUD.cs` | 속도, 연료, 충돌 횟수, 나침반 표시 |
| `Assets/Scripts/MinimapHUD.cs` | 왼쪽 위 미니맵에 장애물 위치 표시 |
| `Assets/Scripts/CheckpointTrigger.cs` | 체크포인트 링 통과 시 타이머 시작 |
| `Assets/Scripts/DestinationTrigger.cs` | 목적지 도착 판정 |
| `Assets/BoatController.cs` | WASD 수동 조종 |
| `Assets/Scripts/Editor/GameSetup.cs` 외 2개 | 에디터 메뉴 `Tools > Setup Simulation Manager / Setup HUD / Setup Checkpoints`로 UI와 체크포인트를 자동 배치 |

## 규칙

- 체크포인트 링을 지나면 타이머가 시작됩니다.
- 연료는 시간이 지나면 줄어들고, 장애물에 부딪히면 추가로 줄어듭니다.
- 시간이 끝나거나 연료가 떨어지면 실패, 그 전에 목적지에 닿으면 성공입니다.

## 조작

| 키 | 동작 |
|---|---|
| Space | 타이틀 화면 넘기기 |
| W / S | 전진 / 후진 |
| A / D | 좌회전 / 우회전 |
| V | 자율운항 켜기/끄기 |
| R | 처음 위치에서 다시 시작 |
| T, F | (테스트용) 체크포인트 통과 / 목적지 도착 처리 |

## 실행 방법

1. Unity **2022.3.62f3** 버전을 설치합니다.
2. 이 저장소는 이미지·모델 파일을 Git LFS로 관리합니다. [Git LFS](https://git-lfs.com)를 설치한 뒤 clone 해야 텍스처가 정상적으로 받아집니다. (ZIP 다운로드로는 LFS 파일이 빠질 수 있습니다.)
   ```
   git lfs install
   git clone https://github.com/kyuyeon76/team6-unity-boat-project.git
   ```
3. Unity Hub에서 `come` 폴더를 프로젝트로 엽니다.
4. `Assets/Scenes/_levels/level_Island.unity` 씬을 열고 실행합니다.
