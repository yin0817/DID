<template>
  <el-dialog
    append-to-body
    :title="isEdit ? '编辑' : '添加'"
    :visible.sync='show'
    :close-on-click-modal="false"
    :before-close='reset'
    width="500px"
  >
    <el-form
      ref="form"
      :model="form.data"
      :rules="form.rules"
      size="small"
      label-width="80px"
    >
      <el-form-item
        label="系统名称"
        prop="name"
      >
        <el-input
          v-model="form.data.name"
          placeholder='请输入系统名称'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="系统类型"
        prop="type"
      >
        <el-select
          v-model="form.data.type"
          clearable
          filterable
          style='width: 100%;'
          placeholder='请选择系统类型'
        >
          <el-option label='1个月' :value='0'>1个月</el-option>
          <el-option label='12个月' :value='1'>12个月</el-option>
        </el-select>
      </el-form-item>
      <el-form-item
        label="系统价格"
        prop="price"
      >
        <el-input
          v-model="form.data.price"
          clearable
          placeholder="请输入系统价格"
        >
          <template slot="prepend">￥</template>
        </el-input>
      </el-form-item>
    </el-form>
    <div
      slot="footer"
      class="dialog-footer"
    >
      <el-button
        type="text"
        @click="toggle(false)"
      >
        取消
      </el-button>
      <el-button
        :loading="form.loading"
        :disabled="form.loading"
        type="primary"
        @click="submit"
      >
        确认
      </el-button>
    </div>
  </el-dialog>
</template>

<script>
import { edit } from '@/api/APP/CLsystem'

export default {
  data() {
    return {
      show: false,
      isEdit: false,
      title: '',
      form: {
        loading: false,
        data: {
          name: '',
          type: '',
          price: ''
        },
        rules: {
          name: [{ required: true, message: '请输入系统名称', trigger: 'blur' }],
          type: [{ required: true, message: '请选择系统类型', trigger: 'blur' }],
          price: [{ required: true, message: '请输入系统价格', trigger: 'blur' }]
        }
      }
    }
  },
  methods: {
    toggle(show, data) {
      this.show = show
      this.isEdit = !!data
      !!data && this.$nextTick().then(() => {
        Object.assign(this.form.data, data)
      })
      !show && this.reset()
    },
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.isEdit = false
      done && done()
    },
    submit() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.form.loading = true
          edit(this.form.data, this.isEdit).then(() => {
            this.$message.success(`${this.isEdit ? '编辑' : '添加'}成功！`)
            this.toggle(false)
            this.$emit('refresh')
          }).catch(() => {
            this.$message.error(`${this.isEdit ? '编辑' : '添加'}失败！`)
          }).finally(() => {
            this.form.loading = false
          })
        }
      })
    }
  }
}
</script>

<style rel="stylesheet/scss" lang="scss" scoped>
 ::v-deep {
   .el-input-number .el-input__inner {
     text-align: left;
   }
   .ql-container {
     height: 300px;
   }
 }
</style>
