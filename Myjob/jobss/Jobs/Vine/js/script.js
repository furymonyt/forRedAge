function init() {
	document.body.style.display = 'block'
	let zone = document.getElementById('zone');
	let spawn = document.getElementById('spawnZone');
	const maxCount = Math.floor(Math.random() * (8 - 5)) + 5
	document.getElementById('count'). innerText = `0`
	document.getElementById('maxCount').innerText = String(maxCount)
	for(let i = 0; i < maxCount; i++) {
		let Vine = document.createElement('div')
		Vine.id = `Vine-${i}`
		Vine.classList.add('Vine')
		document.body.appendChild(Vine);
		const position = getVinePosition(spawn.offsetHeight / 2 - 350, 150, spawn.offsetWidth / 2 - 350, 650)
		Vine.style.position = 'absolute';
		Vine.style.zIndex = 1000;
		Vine.style.left = position.x
		Vine.style.top = position.y

		Vine.onmousedown = function(e) {
			if (Vine.getAttribute("data-inzone") === "true") {
				return
			}

			function moveAt(e) {
				const x = e.pageX - Vine.offsetWidth / 2
				const y = e.pageY - Vine.offsetHeight / 2
				if (x > 0 && x < spawn.offsetWidth - Vine.offsetWidth) {
					Vine.style.left = x + 'px';
				}
				if(y > 0 && y < spawn.offsetHeight - Vine.offsetHeight) {
					Vine.style.top = y + 'px';
				}
			}

			document.onmousemove = function(e) {
				moveAt(e);
			}

			function stop() {
				document.onmousemove = null;
				Vine.onmouseup = null;

				if (Vine.offsetTop > zone.offsetTop && Vine.offsetLeft > zone.offsetLeft && Vine.offsetLeft < zone.offsetLeft + zone.offsetWidth) {
					const count = +zone.getAttribute("data-count") + 1 + ''
					zone.setAttribute("data-count", count)
					document.getElementById('count'). innerText = count
					Vine.setAttribute("data-inzone", "true")
					Vine.style.display = 'none'
					if(+count === maxCount) {
						mp.trigger('closeOpenMenu3', +count)
					}
				}
			}

			Vine.onmouseup = stop

			Vine.ondragstart = function() {
				return false;
			};
		}
	}
}

function getVinePosition(top, height, left, width) {
	return {
		x: Math.floor(Math.random() * width) + left,
		y: Math.floor(Math.random() * height) + top,
	}
}