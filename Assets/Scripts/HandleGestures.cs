using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.UI;

public class HandleGestures : MonoBehaviour
{ 
    public static HandleGestures Instance { get; private set; }

    //Gestures Recognizers for Tap and Navigation Gestures
    GestureRecognizer tapRecognizer;
    GestureRecognizer navigationRecognizer;

    //The application state for dealing with gestures
    private enum AppState
    {
        START,
        TRANSLATE,
        ROTATE,
        SCALE,
        REMOVE_HELMET,
        REMOVE_COILS
    };

    private AppState appState;

    // Use this for initialization
    void Awake()
    {
        Instance = this;

        tapRecognizer = new GestureRecognizer();

        tapRecognizer.Tapped += Recognizer_TappedEvent;

        navigationRecognizer = new GestureRecognizer();

        //Subscribe for input events
        navigationRecognizer.NavigationStartedEvent += NavigationRecognizer_NavigationStartedEvent;
        navigationRecognizer.NavigationUpdatedEvent += NavigationRecognizer_NavigationUpdatedEvent;
        navigationRecognizer.NavigationCompletedEvent += NavigationRecognizer_NavigationCompletedEvent;

        tapRecognizer.StartCapturingGestures();
        navigationRecognizer.StartCapturingGestures();
        appState = AppState.START;
    }

    private void NavigationRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        handleNavigation(normalizedOffset);
    }

    private void NavigationRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        handleNavigation(normalizedOffset);
    }

    private void NavigationRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 normalizedOffset, Ray headRay)
    {
        handleNavigation(normalizedOffset);
    }

    //Handle tap gesture according to the application state
    private void Recognizer_TappedEvent(TappedEventArgs args)
    {
        Text score = GameObject.Find("Text").GetComponent<Text>();
        if (appState == AppState.START)
        {
            score.text = "Translate";
            appState = AppState.TRANSLATE;
        }
        else if (appState == AppState.TRANSLATE)
        {
            score.text = "Rotate";
            appState = AppState.ROTATE;
        }
        else if (appState == AppState.ROTATE)
        {
            score.text = "Scale";
            appState = AppState.SCALE;
        }
        else if (appState == AppState.SCALE)
        {
            ToggleVisibility(GameObject.Find("helmet"));
            score.text = "";
            appState = AppState.REMOVE_HELMET;
        }
        else if (appState == AppState.REMOVE_HELMET)
        {
            ToggleVisibility(GameObject.Find("h4"));
            score.text = "";
            appState = AppState.REMOVE_COILS;
        }
        else if (appState == AppState.REMOVE_COILS)
        {
            ToggleVisibility(GameObject.Find("helmet"));
            ToggleVisibility(GameObject.Find("h4"));
            score.text = "";
            appState = AppState.START;
        }
    }

    //Handle navigation gesture according to the application state
    void handleNavigation(Vector3 normalizedOffset)
    {
        if (appState == AppState.TRANSLATE)
            GameObject.Find("Set").transform.position += GameObject.Find("Set").transform.rotation * normalizedOffset / 50;
        else if (appState == AppState.ROTATE)
            GameObject.Find("Set").transform.Rotate(new Vector3(normalizedOffset.y,normalizedOffset.x,normalizedOffset.z));
        else if (appState == AppState.SCALE)
        {
            if (normalizedOffset.x < 0 && normalizedOffset.y < 0)
                GameObject.Find("Set").transform.localScale *= (1-normalizedOffset.magnitude/30.0f);
            else
                GameObject.Find("Set").transform.localScale *= (1+normalizedOffset.magnitude/30.0f);
        }
    }

    //Change Helmet/Coils visibility 
    void ToggleVisibility(GameObject gameObject_1)
    {
        // toggles the visibility of this gameobject and all it's children
        var renderers = gameObject_1.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = !r.enabled;
        }
    }
}
