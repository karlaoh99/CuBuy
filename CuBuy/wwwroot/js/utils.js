function BuildChart(canvas, label, xdata, ydata) {
    // USE STRICT
    var ctx = document.getElementById(canvas);
    if (ctx) {
      ctx.height = 115;
      var myChart = new Chart(ctx, {
        type: 'bar',
        data: {
          labels: xdata,
          datasets: [
            {
              label: label,
              data:  ydata,
              borderColor: "transparent",
              borderWidth: "0",
              backgroundColor: "rgba(255,255,255,.3)"
            }
          ]
        },
        options: {
          maintainAspectRatio: true,
          legend: {
            display: false
          },
          scales: {
            xAxes: [{
              display: false,
              categoryPercentage: 1,
              barPercentage: 0.65
            }],
            yAxes: [{
              display: false
            }]
          }
        }
      });
    }

}

function Test()
{
    alert("Bazinga!");
}