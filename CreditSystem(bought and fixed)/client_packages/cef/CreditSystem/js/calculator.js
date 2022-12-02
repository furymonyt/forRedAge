function sumProcent() {
    let summ = +document.getElementById("information").value;
    let procentSumm;
    let procent;
    let credit2;
    // Процент
    if (summ <= 200000) {
        procent = 15;
        procentSumm = (summ / 100) * 15;
    }
    if (summ >= 200001) {
        procent = 10;
        procentSumm = (summ / 100) * 10;
    }
    if (summ >= 700001 || summ <= 9999) {
        procent = 0;
        credit2 = 0;
        document.getElementById("procent_system").innerHTML = "процент: " + procent + "%" + "<br>" + "Сумма кредита: " + Math.round(credit2) + "<br>" + "Обновить?";
    } else {
        let credit = summ + procentSumm;
        document.getElementById("procent_system").innerHTML = "процент: " + procent + "%" + "<br>" + "Сумма кредита: " + Math.round(credit) + "<br>" + "Обновить?";
    }
}