# Unity_Sentis_AdaIN

Special thanks to Sky Kim. Visit [Sky's GitHub](https://github.com/skykim).

Original Model: [naoto0804 / pytorch-AdaIN](https://github.com/naoto0804/pytorch-AdaIN)

# Basic Scene

![Result](https://user-images.githubusercontent.com/85833046/230900196-66232d12-bfe4-4d68-bb41-d90adf8022ae.gif)

Double-click the Basic Scene (Assets-Scenes-Basic), and you can add your own textures and run the AdaIN model.

# TTS (Target Texture to Screen Scene)

![img](https://github.com/pnltoen/Unity_Sentis_AdaIN/assets/85833046/099e4a95-e462-4f9f-beff-8a698bfc0a34)

Double-click the TTS Scene (Assets-Scenes-TTS). This scene is designed to use the camera's target texture as the **content input**, and you can render the output to the screen. However, please note that there are some indexing issues because PyTorch usually uses CHW instead of CWH.

- Known Issues:
  - Output resolution is fixed at **512x512**. If you want a different resolution, it would be better to export the ONNX file's shape like this, for example: FullHD (1080x1920). You can edit PyTorch codes or reshape inputs & output tensors using [ShallowReshape](https://docs.unity3d.com/Packages/com.unity.sentis@1.1/manual/do-basic-tensor-operations.html#reshape-a-tensor).
  - The camera needs to be rotated typically **±180** following the Z-axis.

# Articles

For Korean readers, I have published articles on my personal blog. Please read the articles below.

[#1 How To Install Sentis on your Unity project + From Barracuda to Sentis migration guide](https://pnltoen.tistory.com/entry/Unity-Sentis-%ED%8A%9C%ED%86%A0%EB%A6%AC%EC%96%BC-Unity-Sentis-%EC%86%8C%EA%B0%9C-%EB%B0%8F-%EC%84%A4%EC%B9%98-AdaIN-%EC%83%98%ED%94%8C-%EC%86%8C%EA%B0%9C)

[#2 Unity Barracuda vs. Unity Sentis benchmarks](https://pnltoen.tistory.com/entry/Unity-Sentis-%EC%9C%A0%EB%8B%88%ED%8B%B0-%EC%84%BC%ED%8B%B0%EC%8A%A4-%EC%84%B1%EB%8A%A5-%EB%B9%84%EA%B5%90-Feat-%EC%84%B1%EB%8A%A5-%ED%94%84%EB%A1%9C%ED%8C%8C%EC%9D%BC%EB%A7%81%EA%B3%BC-%EC%B5%9C%EC%A0%81%ED%99%94-%EB%A6%AC%EB%B7%B0)

[#3 Let's apply StyleTransfer (AdaIN) to New3DURPSample](https://pnltoen.tistory.com/entry/Unity-Sentis-%EC%9C%A0%EB%8B%88%ED%8B%B0-%EC%84%BC%ED%8B%B0%EC%8A%A4%EB%A5%BC-%ED%99%9C%EC%9A%A9%ED%95%98%EC%97%AC-%EC%83%88%EB%A1%9C%EC%9A%B4-URP-%EC%83%98%ED%94%8C%EC%97%90-StyleTransfer-AdaIN-%EC%A0%81%EC%9A%A9%EA%B8%B0)

🚨 Please note that I do not hold any copyrights for the sources used in this project. 🚨
