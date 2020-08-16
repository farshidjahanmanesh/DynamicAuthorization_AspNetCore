
function ViewerVisitorGraph(model) {
	var options = {
		chart: {
			height: 300,
			type: 'area',
			zoom: {
				enabled: false
			}
		},
		dataLabels: {
			enabled: false
		},
		stroke: {
			curve: 'straight',
			width: 3,
		},
		series: [{
			name: "آمار بازدید در روز :",
			data: model.Counts
			//data: series.monthDataSeries.prices
		}],
		title: {
			text: 'آمار بازدید کاربران',
			align: 'center'
		},
		grid: {
			row: {
				colors: ['#ffffff'], // takes an array which will be repeated on columns
				opacity: 0.5
			},
		},
		labels: model.Dates,
		xaxis: {
			type: 'datetime',
		},
		yaxis: {
			opposite: true
		},
		legend: {
			horizontalAlign: 'left'
		},
		theme: {
			monochrome: {
				enabled: true,
				color: '#1a8e5f',
				shadeIntensity: 0.1
			},
		},
		markers: {
			size: 0,
			opacity: 0.2,
			colors: ["#1a8e5f"],
			strokeColor: "#fff",
			strokeWidth: 2,
			hover: {
				size: 7,
			}
		},
	}

	var chart = new ApexCharts(
		document.querySelector("#basic-area-graph"),
		options
	);

	chart.render();
}


function DeviceChecker(model) {

	Morris.Donut({
		element: 'donutColors',
		data: [
			{ value: model.IOS, label: 'IOS' },
			{ value: model.Android, label: 'Android' },
			{ value: model.Desktop, label: 'Desktop' },
			{ value: model.Other, label: 'Other' }
		],
		backgroundColor: '#ffffff',
		labelColor: '#666666',
		colors: ['#1a8e5f', '#262b31', '#434950', '#63686f', '#868a90'],
		resize: true,
		hideHover: "auto",
		height: 400,
		gridLineColor: "#e4e6f2",
		formatter: function (x) { return x + "%" }
	});
}


function BrowserGraph(model) {
	var options = {
		chart: {
			width: 400,
			type: 'donut',
		},
		series: [model.FireFox, model.Safari, model.Chorome, model.Edge, model.IE, model.Other],
		labels: ["FireFox", "Safari", "Chorome", "Edge", "IE", "Other"],
		fill: {
			type: 'gradient',
		},
		theme: {
			monochrome: {
				enabled: true,
				color: '#1a8e5f',
			}
		},
		title: {
			text: "آمار ماهیانه",
		},
		responsive: [{
			breakpoint: 480,
			options: {
				chart: {
					width: 200
				},
				legend: {
					position: 'bottom'
				}
			}
		}],
		stroke: {
			width: 0,
		},
	}
	var chart = new ApexCharts(
		document.querySelector("#basic-donut-graph-monochrome-gradient"),
		options
	);
	chart.render();

}