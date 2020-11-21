let ctx;

function initCanvas(canvas_ref) {
	ctx = canvas_ref.getContext('2d');
}


function drawCol(i, h, colors) {
	ctx.strokeStyle = colors[(h.d * 4) >> 0];

	let v = 0;
	if (h.d != 0)
		v = 256 / h.d;

	ctx.beginPath();
	ctx.moveTo(i, 128 - (v / 2));
	ctx.lineTo(i, 128 + (v / 2));
	ctx.stroke();
};


function render(rayBuffer, colors) {
	ctx.fillRect(0, 0, view.width, view.height);
	
	for (let index = 0; index < 256; index++) {
		drawCol(index, rayBuffer[index], colors);
	}
};