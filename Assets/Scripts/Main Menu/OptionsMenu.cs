using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour {


    public Dropdown resolutionDropdown;//Dropdown obj
    int resolutionindex = 0;//index de resol.
    Resolution[] resolution;//vetor de resol.

    void Start()
    {
        resolution = Screen.resolutions;//o vetor adiquiri as resolucoes posiveis 
        resolutionDropdown.ClearOptions();//lismpa o dropdown

        List<string> options = new List<string>();//cria uma lista strings para o dropdown

        for (int i = 0; i < resolution.Length; i++)
        {
            string option = resolution[i].width + " X " + resolution[i].height;// string no formato [widht X height]
            options.Add(option);//adiciona a nova opcao na string

            if (resolution[i].height == Screen.currentResolution.height && resolution[i].width == Screen.currentResolution.width)
            {
                resolutionindex = i;//indica qual e o index da resolucao atual
            }
        }

        resolutionDropdown.AddOptions(options);//adiciona as novas opcoes
        resolutionDropdown.value = resolutionindex;//adiciona a selecao atual
        resolutionDropdown.RefreshShownValue();//atualiza
    }

    //muda a resolucao pelo index selecionado
    public void SetResolution(int index)
    {
        Resolution newresolution = resolution[index];//seleciona com base no index
        Screen.SetResolution(newresolution.width, newresolution.height, Screen.fullScreen);//aplica a nova resolucao
    }

    //entra e sai de FullScreen
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetQuality(int qualityindex)//seleciona a qualidade de a cordo com o index atual aplicando ele no sistema base da unity
    {
        QualitySettings.SetQualityLevel(qualityindex);
    }
}
