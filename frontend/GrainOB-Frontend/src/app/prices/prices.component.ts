import { Component, OnInit } from '@angular/core';
import { PricesService } from '../prices.service';
import { Chart, registerables } from 'chart.js';

@Component({
  selector: 'app-prices',
  templateUrl: './prices.component.html',
  styleUrls: ['./prices.component.css']
})
export class PricesComponent implements OnInit {
  prices: any[] = [];
  chart: Chart | undefined;
  selectedGrainType: string = 'Wheat';
  selectedGrainClass: string = 'Class 1';
  latestPrice: number | null = null;

  constructor(private pricesService: PricesService) {
    Chart.register(...registerables);
  }

  ngOnInit() {
    this.onTabChange(this.selectedGrainType, this.selectedGrainClass);
  }  

  onTabChange(grainType: string, grainClass?: string) {
    this.selectedGrainType = grainType;
    this.selectedGrainClass = grainClass ?? ''; // Use nullish coalescing operator
    this.loadPricesForGrain(grainType, this.selectedGrainClass);
  }

  loadPricesForGrain(grainType: string, grainClass: string) {
    this.pricesService.getPricesByGrainTypeAndClass(grainType, grainClass).subscribe(data => {
      // Assuming the server's timestamps are in UTC and you want to compare against UTC
      const currentTime = new Date().getTime();
  
      let recentPrices = data.filter(p => {
        // Parse the server timestamp as UTC
        const serverTime = new Date(p.timestamp + 'Z').getTime(); // Add 'Z' to indicate UTC
        return currentTime - serverTime <= 2 * 60 * 60 * 1000; // 2 hours in milliseconds
      });
  
      if (recentPrices.length > 0) {
        this.prices = recentPrices;
        this.latestPrice = recentPrices[recentPrices.length - 1].price;
        this.loadChart();
      } else {
        console.warn('No recent prices found within the last 2 hours.');
      }
    }, error => {
      console.error('Failed to load prices:', error);
    });
  }
  
  getChartTitle(): string {
    let classDisplay = this.selectedGrainClass !== 'None' ? `${this.selectedGrainClass}` : '';
    let latestPriceDisplay = this.latestPrice !== null ? `Current price: ${this.latestPrice} €/t` : 'Current price: N/A';
    return `${this.selectedGrainType} ${classDisplay} Prices. ${latestPriceDisplay}`;
  }

  loadChart() {
    if (this.chart) {
      this.chart.destroy();
    }
    const canvas = <HTMLCanvasElement>document.getElementById('myChart');
    const ctx = canvas.getContext('2d');
    if (!ctx) {
      console.error('Could not get context from canvas');
      return;
    }
  
    // Calculate the percentage change for each data point
    let percentageChangeData = [];
    for (let i = 1; i < this.prices.length; i++) {
      let oldPrice = this.prices[i - 1].price;
      let newPrice = this.prices[i].price;
      let change = ((newPrice - oldPrice) / oldPrice) * 100;
      percentageChangeData.push(change);
    }
  
    // Add an initial value for the first data point
    percentageChangeData.unshift(0);
  
    this.chart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: this.prices.map(p => new Date(p.timestamp).toLocaleString('en-US', {
          year: 'numeric', month: '2-digit', day: '2-digit',
          hour: '2-digit', minute: '2-digit', hour12: false
        })),
        datasets: [{
          label: 'Price',
          data: this.prices.map(p => p.price),
          backgroundColor: '#005f54',
          borderColor: '#003a33',
          borderWidth: 1,
          yAxisID: 'y-axis-1', // Set yAxisID for the first dataset
          order: 2,
        }, {
          label: 'Percentage Change',
          data: percentageChangeData,
          type: 'line',
          fill: false,
          borderColor: '#B8860B',
          borderWidth: 2,
          pointRadius: 0,
          yAxisID: 'y-axis-2', // Set yAxisID for the second dataset
          order: 1,
        }]
      },
      options: {
        maintainAspectRatio: false,
        scales: {
          'y-axis-1': { // Configure the first y-axis
            type: 'linear',
            display: true,
            position: 'left',
          },
          'y-axis-2': { // Configure the second y-axis for percentage change
            type: 'linear',
            display: true,
            position: 'right',
            grid: {
              drawOnChartArea: false,
            },
          },
        },
        plugins: {
          title: {
            display: true,
            text: this.getChartTitle(),
            font: {
              size: 24
            },
            padding: {
              top: 10,
              bottom: 30
            }
          }
        }
      }
    });
  }
}
