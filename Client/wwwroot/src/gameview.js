let ctx;

let width;

let height;

function initCanvas(canvas_ref) {
	ctx = canvas_ref.getContext('2d');
	width = canvas_ref.width;
	height = canvas_ref.height;
}


function drawCol(i, h, colors) {
	//console.log("DRAWING COLUMN");
	ctx.strokeStyle = colors[(h * 4) >> 0];
	let v = 0;
	if (h != 0)
		v = 256 / h;

	ctx.beginPath();
	ctx.moveTo(i, 128 - (v / 2));
	ctx.lineTo(i, 128 + (v / 2));
	ctx.stroke();
};


function render(rayBuffer, colors) {
	//console.log("RENDERING FRAME");
	ctx.fillRect(0, 0, width, height);
	
	for (let index = 0; index < 256; index++) {
		drawCol(index, rayBuffer[index], colors);
	}
};