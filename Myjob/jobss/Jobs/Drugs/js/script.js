function init() {
	document.body.style.display = 'block'
	let zone = document.getElementById('zone');
	let spawn = document.getElementById('spawnZone');
	const maxCount = Math.floor(Math.random() * (8 - 5)) + 5
	document.getElementById('count'). innerText = `0`
	document.getElementById('maxCount').innerText = String(maxCount)
	for(let i = 0; i < maxCount; i++) {
		let Drug = document.createElement('div')
		Drug.id = `Drug-${i}`
		Drug.classList.add('Drug')
		document.body.appendChild(Drug);
		const position = getDrugPosition(spawn.offsetHeight / 2 - 350, 150, spawn.offsetWidth / 2 - 350, 650)
		Drug.style.position = 'absolute';
		Drug.style.zIndex = 1000;
		Drug.style.left = position.x
		Drug.style.top = position.y

		Drug.onmousedown = function(e) {
			if (Drug.getAttribute("data-inzone") === "true") {
				return
			}

			function moveAt(e) {
				const x = e.pageX - Drug.offsetWidth / 2
				const y = e.pageY - Drug.offsetHeight / 2
				if (x > 0 && x < spawn.offsetWidth - Drug.offsetWidth) {
					Drug.style.left = x + 'px';
				}
				if(y > 0 && y < spawn.offsetHeight - Drug.offsetHeight) {
					Drug.style.top = y + 'px';
				}
			}

			document.onmousemove = function(e) {
				moveAt(e);
			}

			function stop() {
				document.onmousemove = null;
				Drug.onmouseup = null;

				if (Drug.offsetTop > zone.offsetTop && Drug.offsetLeft > zone.offsetLeft && Drug.offsetLeft < zone.offsetLeft + zone.offsetWidth) {
					const count = +zone.getAttribute("data-count") + 1 + ''
					zone.setAttribute("data-count", count)
					document.getElementById('count'). innerText = count
					Drug.setAttribute("data-inzone", "true")
					Drug.style.display = 'none'
					if(+count === maxCount) {
						mp.trigger('closeOpenMenu2', +count)
					}
				}
			}

			Drug.onmouseup = stop

			Drug.ondragstart = function() {
				return false;
			};
		}
	}
}

function getDrugPosition(top, height, left, width) {
	return {
		x: Math.floor(Math.random() * width) + left,
		y: Math.floor(Math.random() * height) + top,
	}
}