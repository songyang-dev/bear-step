using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Events;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Links the objects in the level scene with correct references
/// </summary>
public class LevelPrefabLinker : Editor
{
    private const string levelPrefabPath = "Assets/Prefabs/LevelGameObjects.prefab";

    static private GameObject root;
    private static GameObject gameManager;

    /// <summary>
    /// Event to trigger when all basic linking of references are done
    /// </summary>
    private static event Action LinkEvents;

    /// <summary>
    /// Links all the references and sets all inspector values
    /// </summary>
    [MenuItem("Prefabs/Link Level")]
    public static void SetAllReferences()
    {
        SetUp();

        TraverseHierarchy(root);
        LinkEvents.Invoke();

        CleanUp();
    }

    /// <summary>
    /// Traverses the game object in the prefab hierarchy
    /// </summary>
    /// <param name="node">Game object to examine</param>
    private static void TraverseHierarchy(GameObject node)
    {
        // Decide which method to call next based on the name of the game object
        switch (node.name)
        {
            case "LevelGameObjects":
                TraverseParent(node);
                break;

            case "Main Camera":
                TraverseCamera(node);
                break;

            case "Game Manager":
                TraverseGameManager(node);
                break;

            case "Board":
                TraverseBoard(node);
                break;

            case "Game UI":
                TraverseParent(node);
                break;

            case "Orb Count Text":
                TraverseOrbCount(node);
                break;

            case "Orb Message":
                TraverseOrbMessage(node);
                break;

            case "Button":
                TraverseButton(node);
                break;

            case "Pause Menu":
                TraversePauseMenu(node);
                break;

            case "Pause Menu UI":
                TraverseParent(node);
                break;

            case "Resume Button":
            case "Restart Button":
                TraverseButton(node);
                break;

            case "Win Menu":
                TraverseWinMenu(node);
                break;

            case "Win Menu UI":
                TraverseParent(node);
                break;

            case "Next Button":
            case "Quit Button":
                TraverseButton(node);
                break;

            // Do not traverse if no case is present for the given name
            default:
                return;
        }
    }

    /// <summary>
    /// Sets the win menu script
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseWinMenu(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<WinMenuUI>());

        var script = node.AddComponent<WinMenuUI>();

        script.GameManager = gameManager;

        TraverseParent(node);
    }

    /// <summary>
    /// Sets the pause menu script
    /// </summary>
    /// <param name="node"></param>
    private static void TraversePauseMenu(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<PauseMenu>());

        var script = node.AddComponent<PauseMenu>();
        script.GameIsPaused = false;
        script.GameManager = gameManager;

        TraverseParent(node);
    }

    /// <summary>
    /// Attaches the correct onClick listener to the given button
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseButton(GameObject node)
    {
        var button = node.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();

        switch (node.transform.parent.name)
        {
            case "Game UI":
                LinkEvents += () =>
                {
                    var action = Delegate.CreateDelegate(
                        typeof(UnityAction),
                        root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                        "Pause"
                    ) as UnityAction;
                    UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                };
                break;

            case "Pause Menu UI":
                switch (node.name)
                {
                    case "Resume Button":
                        LinkEvents += () =>
                        {
                            var action = Delegate.CreateDelegate(
                                typeof(UnityAction),
                                root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                                "Resume"
                            ) as UnityAction;
                            UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                        };
                        break;

                    case "Restart Button":
                        LinkEvents += () =>
                        {
                            var action = Delegate.CreateDelegate(
                                typeof(UnityAction),
                                root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                                "Restart"
                            ) as UnityAction;
                            UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                        };
                        break;

                    case "Quit Button":
                        LinkEvents += () =>
                        {
                            var action = Delegate.CreateDelegate(
                                typeof(UnityAction),
                                root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                                "Menu"
                            ) as UnityAction;
                            UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                        };
                        break;

                    default:
                        break;
                }
                break;

            case "Win Menu UI":
                switch (node.name)
                {
                    case "Next Button":
                        LinkEvents += () =>
                        {
                            var action = Delegate.CreateDelegate(
                                typeof(UnityAction),
                                root.transform.Find("Win Menu").GetComponent<WinMenuUI>(),
                                "NextLevel"
                            ) as UnityAction;
                            UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                        };
                        break;

                    case "Quit Button":
                        LinkEvents += () =>
                        {
                            var action = Delegate.CreateDelegate(
                                typeof(UnityAction),
                                root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                                "Menu"
                            ) as UnityAction;
                            UnityEventTools.AddVoidPersistentListener(button.onClick, action);
                        };
                        break;

                    default:
                        break;
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Sets the Orb Message script and the animator
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseOrbMessage(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<OrbMessageUI>());

        // script
        var script = node.AddComponent<OrbMessageUI>();
        script.GameManager = gameManager;

        // animator
        var animator = node.GetComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/Orb Message.controller");

    }

    /// <summary>
    /// Adds the orb count script
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseOrbCount(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<OrbCountUI>());

        var script = node.AddComponent<OrbCountUI>();
        script.HasWon = false;
    }

    /// <summary>
    /// Adds the board script and sets its values
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseBoard(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<Board>());

        var board = node.AddComponent<Board>();

        // references
        board.GameManager = gameManager;
        board.flipDistance = 1;
        board.flipDuration = 1;
        // events
        board.TouchOrb = new UnityEvent();
        LinkEvents += () =>
        {
            var action = Delegate.CreateDelegate(
                typeof(UnityAction),
                root.transform.Find("Board").GetComponent<Board>(),
                "DisplayOrbMessage"
            ) as UnityAction;
            UnityEventTools.AddVoidPersistentListener(board.TouchOrb, action);
        };
    }

    /// <summary>
    /// Adds the game manager script and sets its values, also sets the
    /// player input component
    /// </summary>
    /// <param name="node">Game manager game object</param>
    private static void TraverseGameManager(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        if (count == 0) DestroyImmediate(node.GetComponent<GameManager>());

        // script
        var gameManager = node.AddComponent<GameManager>();

        // references
        gameManager.LevelScene = "Level";
        gameManager.MenuScene = "Main Menu";
        gameManager.Board = root.transform.Find("Board").gameObject;
        gameManager.Cam = root.transform.Find("Main Camera").gameObject;
        gameManager.Test = true;
        gameManager.TestLevel = "Campaign/Tutorial";
        gameManager.BearPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bear.prefab");
        gameManager.OrbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Orb.prefab");
        gameManager.TilePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tile.prefab");
        gameManager.OrbMessageUIAnimator = root.transform.Find("Game UI").Find("Orb Message").GetComponent<Animator>();
        gameManager.OrbCountUI = root.transform.Find("Game UI").Find("Orb Count Text").gameObject;
        gameManager.OrbMessageUI = root.transform.Find("Game UI").Find("Orb Message").gameObject;
        gameManager.PauseMenuUI = root.transform.Find("Pause Menu").Find("Pause Menu UI").gameObject;
        gameManager.WinMenuUI = root.transform.Find("Win Menu").Find("Win Menu UI").gameObject;
        gameManager.NextLevelButton = root.transform.Find("Win Menu").Find("Win Menu UI").Find("Next Button").gameObject;
        // events
        gameManager.WinLevel = new UnityEvent();
        LinkEvents += () =>
        {
            var action = Delegate.CreateDelegate(
                typeof(UnityAction),
                root.transform.Find("Win Menu").GetComponent<WinMenuUI>(),
                "Show"
            ) as UnityAction;
            UnityEventTools.AddVoidPersistentListener(gameManager.WinLevel, action);
        };

        // player input (Input system 1.0.0)
        var playerInput = gameManager.GetComponent<PlayerInput>();

        // settings
        playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/Scripts/Level/controls.inputactions");
        playerInput.defaultControlScheme = "Keyboard&Mouse";
        playerInput.defaultActionMap = "Player";
        playerInput.notificationBehavior = PlayerNotifications.InvokeUnityEvents;
        // events
        foreach (var item in playerInput.actionEvents)
        {
            // name of events are like this: Player/Center[/Keyboard/c]
            // so only take the part before []
            var eventName = item.actionName.Split('[')[0];

            // remove all of its listeners
            try
            {
                for (int i = 0; ; i++)
                {
                    UnityEventTools.RemovePersistentListener(item, i);
                }
            }
            catch (ArgumentOutOfRangeException) { }

            switch (eventName)
            {
                case "Player/Move":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Board").GetComponent<Board>(),
                            "MoveAllPlayers"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                case "Player/Look":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Main Camera").GetComponent<CameraController>(),
                            "Move"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                case "Player/Center":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Main Camera").GetComponent<CameraController>(),
                            "Center"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                case "Player/Flip":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Board").GetComponent<Board>(),
                            "Flip"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                case "Player/Pause":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                            "Pause"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                case "Paused/Resume":
                    LinkEvents += () =>
                    {
                        var action = Delegate.CreateDelegate(
                            typeof(UnityAction<InputAction.CallbackContext>),
                            root.transform.Find("Pause Menu").GetComponent<PauseMenu>(),
                            "Resume"
                        ) as UnityAction<InputAction.CallbackContext>;
                        UnityEventTools.AddPersistentListener(item, action);
                    };
                    break;

                // ignore other events
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Adds the camera controller script and sets the inspector values
    /// </summary>
    /// <param name="node">Camera game object</param>
    private static void TraverseCamera(GameObject node)
    {
        var count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(node);

        // add camera controller script back if needed
        if (count == 0) DestroyImmediate(node.GetComponent<CameraController>());

        var controller = node.AddComponent<CameraController>();

        controller.Offset = new Vector3(0.18f, -1.86f, -5.08f);
        controller.Rotation = new Vector3(-37.233f, 0, 0);
        controller.Speed = 3f;
        controller.gameManager = gameManager;

    }

    /// <summary>
    /// Traverses the parent to only call its children
    /// </summary>
    /// <param name="node"></param>
    private static void TraverseParent(GameObject node)
    {
        for (int i = 0; i < node.transform.childCount; i++)
        {
            var child = node.transform.GetChild(i);
            TraverseHierarchy(child.gameObject);
        }
    }

    private static void SetUp()
    {
        root = PrefabUtility.LoadPrefabContents(levelPrefabPath);
        gameManager = root.transform.Find("Game Manager").gameObject;
    }

    private static void CleanUp()
    {
        PrefabUtility.SaveAsPrefabAsset(root, levelPrefabPath);
        PrefabUtility.UnloadPrefabContents(root);
    }
}
