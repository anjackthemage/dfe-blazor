let game_canvas;
let ctx;
let width;
let height;
let mouseDelta = 0;

function initCanvas(canvas_ref) {
	game_canvas = canvas_ref;
	ctx = canvas_ref.getContext('2d');
	width = canvas_ref.width;
	height = canvas_ref.height;

	window.addEventListener('pointerlockchange', lockMouse, false);
}

let screenImageData;
function blitScreen(pixelData, width, height) {
	if (!screenImageData) {
		screenImageData = new ImageData(width, height);
	}
	screenImageData.data.set(Blazor.platform.toUint8Array(pixelData));
	ctx.putImageData(screenImageData, 0, 0);
};

let pointerLocked = false; 
function captureMouse(pointerId) {
	pointerId.requestPointerLock = pointerId.requestPointerLock ||
								   pointerId.mozRequestPointerLock ||
								   pointerId.webkitRequestPointerLock;
	pointerId.requestPointerLock();
};

function pollMouse() {
	if (pointerLocked === false)
		return 0;
	else
		return mouseDelta;
};

function lockMouse() {
	if (document.pointerLockElement === game_canvas) {
		pointerLocked = true;
		game_canvas.onmousemove = handleMouse;
		console.log("pointer locked");
	} else {
		mouseDelta = 0;
		pointerLocked = false;
		console.log("pointer unlocked");
    }
};

function handleMouse(me_args) {
	mouseDelta = me_args.movementX;
};