

function setAllAudioMute(bIsMuted) {
    Array.from(document.getElementsByTagName("Audio")).forEach((element) => {
        element.muted = bIsMuted;
    });
}

function playAudio (uri) {
    var audio = new Audio(uri);

    audio.addEventListener("canplay", (event) => {
        audio.play();
    });

    audio.loop = true;
    audio.load();

    document.getElementById("audio-elements").appendChild(audio);
};