<template>
  <div>
    <el-form
      ref="form"
      :inline="true"
      :model="form"
      size="small"
      label-width="66px"
    >
      <el-form-item label="年份">
        <el-date-picker v-model="form.year" type="year" placeholder="选择日期" style="width: 120px" />
      </el-form-item>
    </el-form>
    <el-tabs v-model="activeName" type="card" @tab-click="handleClick">
      <el-tab-pane label="日K" name="rk">
        <div id="rk" :class="className" :style="{height:height,width:width}" />
      </el-tab-pane>
      <el-tab-pane label="周K" name="zk">
        <div id="zk" :class="className" :style="{height:height,width:width}" />
      </el-tab-pane>
      <el-tab-pane label="月K" name="yk">
        <div id="yk" :class="className" :style="{height:height,width:width}" />
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import echarts from 'echarts'
require('echarts/theme/macarons') // echarts theme

export default {
  components: {
  },
  props: {
    className: {
      type: String,
      default: 'chart'
    },
    width: {
      type: String,
      default: '100%'
    },
    height: {
      type: String,
      default: '300px'
    }
  },
  data() {
    return {
      chart: null,
      form: {
        year: new Date().getFullYear().toString()
      },
      activeName: 'rk'
    }
  },
  mounted() {
    this.initRKChart()
    this.initZKChart()
    this.initYKChart()
  },
  methods: {
    handleClick(tab, event) {
      console.log(tab, event)
    },
    initRKChart() {
      this.chart = echarts.init(document.getElementById('rk'))

      this.chart.setOption({
        xAxis: {
          data: ['2017-10-24', '2017-10-25', '2017-10-26', '2017-10-27']
        },
        yAxis: {},
        series: [
          {
            type: 'candlestick',
            data: [
              [20, 34, 10, 38],
              [40, 35, 30, 50],
              [31, 38, 33, 44],
              [38, 15, 5, 42]
            ]
          }
        ]
      })
    },
    initZKChart() {
      var myChart = document.getElementById('zk')
      myChart.style.width = document.getElementById('rk').firstElementChild.style.width
      this.chart = echarts.init(myChart)

      this.chart.setOption({
        xAxis: {
          data: ['2017-10-24', '2017-10-25', '2017-10-26', '2017-10-27']
        },
        yAxis: {},
        series: [
          {
            type: 'candlestick',
            data: [
              [20, 34, 10, 38],
              [40, 35, 30, 50],
              [31, 38, 33, 44],
              [38, 15, 5, 42]
            ]
          }
        ]
      })
    },
    initYKChart() {
      var myChart = document.getElementById('yk')
      myChart.style.width = document.getElementById('rk').firstElementChild.style.width
      this.chart = echarts.init(myChart)

      this.chart.setOption({
        xAxis: {
          data: ['2017-10-24', '2017-10-25', '2017-10-26', '2017-10-27']
        },
        yAxis: {},
        series: [
          {
            type: 'candlestick',
            data: [
              [20, 34, 10, 38],
              [40, 35, 30, 50],
              [31, 38, 33, 44],
              [38, 15, 5, 42]
            ]
          }
        ]
      })
    }
  }
}
</script>
