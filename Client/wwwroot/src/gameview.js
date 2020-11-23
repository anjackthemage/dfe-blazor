let ctx;
let width;
let height;

function initCanvas(canvas_ref) {
	console.log("-- InitCanvas --");
	ctx = canvas_ref.getContext('2d');
	width = canvas_ref.width;
	height = canvas_ref.height;
	console.log(ctx);
}

var screenImageData;
function blitScreen(pixelData, width, height) {
	if (!screenImageData) {
		screenImageData = new ImageData(width, height);
	}
	screenImageData.data.set(Blazor.platform.toUint8Array(pixelData));
	ctx.putImageData(screenImageData, 0, 0);
};