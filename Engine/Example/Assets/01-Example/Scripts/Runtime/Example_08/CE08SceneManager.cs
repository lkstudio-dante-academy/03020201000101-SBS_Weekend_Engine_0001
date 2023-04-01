using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 유니티 UI 시스템이 정상적으로 구동되기 위해서는 반드시 씬 상에 캔버스와 이벤트 시스템 컴포넌트를 지니고 있는 게임 객체 
 * 존재해야한다. (즉, 해당 컴포넌트 중 하나라도 존재하지 않을 경우 유니티 UI 는 정상적으로 동작하지 않는다는 것을 알 수 있다.)
 * 
 * 캔버스는 유니티가 지원하는 여러 UI 컴포넌트가 실질적으로 배치되는 도화지 역할이다. 따라서, 유니티의 모든 UI 는 캔버스 상에 
 * 배치되고 그려지며 해당 캔버스는 필요에 따라 2 개 이상 배치하는 것이 가능하다.
 * 
 * 즉, 모든 UI 는 특정 캔버스 하위에 존재해야되는 제안이 있다는 것을 알 수 있다.
 * 
 * 이벤트 시스템은 사용자와 유니티 UI 가 상호작용 할 수 있도록 처리해주는 역할을 수행한다. 따라서, 해당 컴포넌트가 존재하지 않는다면
 * 사용자의 입력에 유니티 UI 가 반응하지 않기 때문에 버튼 등의 사용자 입력이 필요한 UI 컴포넌트가 정상적으로 구동되지 않는다는 것을
 * 알 수 있다.
 * 
 * 단, 이벤트 시스템은 캔버스와 달리 씬 상에 중복적으로 존재 할 경우 정상적으로 구동되지 않기 때문에 씬에는 반드시 하나의
 * 이벤트 시스템만 존재해야한다. 
 * 
 * 따라서, 여러 씬을 중첩적으로 사용 할 경우 이벤트 시스템 여러 개 존재 할 수 있는 문제점이 발생 할 수 있기 때문에 씬을 중첩으로 사용
 * 할 경우에는 Active 씬을 제외하고는 모두 이벤트 시스템 컴포넌트를 제거하거나 비활성해줘야한다.
 * 
 * 또한, 유니티는 사용자 입력에 해당하는 UI 컴포넌트를 식별하기 위해서 내부적으로 Raycast 방식을 사용하기 때문에 사용자 입력을 처리
 * 하기 위해서는 이벤트 시스템과 더블어 Raycaster 계열 컴포넌트가 반드시 필요하다.
 * 
 * 유니티 Raycaster 컴포넌트 종류
 * - Graphic Raycaster		<- UI 컴포넌트용
 * - 2D Raycaster			<- 2D 객체용 (Ex. 스프라이트 등등...)
 * - 3D Raycaster			<- 3D 객체용 (Ex. 큐브 등등...)
 * 
 * 일반적으로 UI 상호작용 위해서는 Graphic Raycaster 만 사용하면 되지만 2D 또는 3D 객체를 터치 등을 통해 구별하고 싶을 경우
 * 2D/3D Raycaster 를 활용하면 유니티 UI 시스템에 의해서 해당 객체를 구별하는 것이 가능하다. (즉, 유니티 UI 시스템과 상호작용을
 * 할 수 있다는 것을 의미한다.)
 * 
 * 유니티 UI 좌표 시스템은 크게 앵커와 피봇을 통해서 각 UI 의 위치를 계산한다.
 * 앵커는 부모 UI 영역 중 어떤 곳을 기준으로 위치를 계산 할지 결정하는데 사용되며 피봇은 특정 UI 자신의 기준점을 의미한다.
 * 
 * 따라서, 유니티 UI 는 앵커와 피봇을 이용해서 캔버스 상에 배치 될 위치가 결정되며 해당 기능들을 활용하면 여러 해상도를 손쉽게
 * 처리하는 것이 것이 가능하다. (즉, 해상도 크기에 상관 없이 특정 UI 를 항상 동일한 위치에 배치하는 등의 기능을 구현하는 것이
 * 가능하다.)
 * 
 * Rect Transform 이란?
 * - 일반적으로 유니티 UI 를 캔버스 상에 배치하기 위해서 사용되는 Transform 계열 컴포넌트를 의미하며 해당 컴포넌트를 활용하면
 * 앵커와 피봇 같은 유니티 UI 시스템에 맞는 정보를 설정하는 것이 가능하다.
 * 
 * 또한, Rect Transform 은 Transform 컴포넌트를 상속해서 구현되어 있기 때문에 기존에 Transform 컴포넌트가 지니고 있는 기능을
 * 모두 사용하는 것이 가능하다.
 * 
 * 따라서, 대부분의 UI 컴포넌트는 Rect Transform 컴포넌트를 좀더 쉽게 활용하기 위해서 해당 컴포넌트를 가져 올 수 있는 rectTransform
 * 프로퍼티를 제공한다. (즉, transform 프로퍼티와 동일한 목적을 가지고 있다는 것을 알 수 있다.)
 */
/** Example 8 */
public class CE08SceneManager : CSceneManager {
	#region 변수
	[SerializeField] private CPopup m_oPopup = null;
	#endregion // 변수

	#region 프로퍼티
	public override string SceneName => KDefine.G_SCENE_N_E08;
	#endregion // 프로퍼티

	#region 함수
	/** 초기화 */
	public override void Awake() {
		base.Awake();
	}

	/** 상태를 갱신한다 */
	public void Update() {
		// ESC 키를 눌렀을 경우
		if(Input.GetKeyDown(KeyCode.Escape)) {
			CFunc.ShowPopup("Popup", this.PopupUIs, "Global/Prefabs/G_Popup", null);
		}
	}
	#endregion // 함수
}
