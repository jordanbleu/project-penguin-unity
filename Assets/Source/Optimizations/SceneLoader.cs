using System;
using System.Collections;
using System.Collections.Generic;
using Source.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Source.Optimizations
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private GameObject fadeToBlack;

        [SerializeField]
        private GameObject loadingScreen;

        private string _sceneId;
        private void Start()
        {
            fadeToBlack.SetActive(false);
            loadingScreen.SetActive(false);
            
            var fadeToBlackSpriteAnimator = fadeToBlack.GetComponent<SpriteAnimator>();
            fadeToBlackSpriteAnimator.OnIterationComplete.AddListener(OnFadeToBlackComplete);
        }
        
        // begins fading to the loading screen
        public void BeginFadingToScene(string sceneId)
        {
            this.enabled = true;
            fadeToBlack.SetActive(true);
            _sceneId = sceneId;
        }
        
        
        private void OnFadeToBlackComplete()
        {
            // trigger loading screen
            loadingScreen.SetActive(true);
            
            StartCoroutine(BeginLoadingScene(_sceneId));

            fadeToBlack.SetActive(false);
        }
        
        private IEnumerator BeginLoadingScene(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

    }
}