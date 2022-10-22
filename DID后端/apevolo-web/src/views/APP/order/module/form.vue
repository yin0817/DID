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
      label-width="90px"
    >
      <el-form-item
        label="关联编号"
        prop="rid"
      >
        <el-input
          v-model="form.data.rid"
          placeholder='请输入关联编号'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="didUserId"
        prop="didUserId"
      >
        <el-input
          v-model="form.data.didUserId"
          placeholder='请输入didUserId'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="订单类型"
        prop="orderType"
      >
        <el-select
          v-model="form.data.orderType"
          clearable
          filterable
          style='width: 100%;'
          placeholder='请选择订单类型'
        >
          <el-option label='课程' :value='0'>课程</el-option>
          <el-option label='系统' :value='1'>系统</el-option>
        </el-select>
      </el-form-item>
      <el-form-item
        label="用户名称"
        prop="name"
      >
        <el-input
          v-model="form.data.name"
          placeholder='请输入用户名称'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="手机号码"
        prop="phone"
      >
        <el-input
          v-model="form.data.phone"
          placeholder='请输入手机号码'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="微信号"
        prop="wechat"
      >
        <el-input
          v-model="form.data.wechat"
          placeholder='请输入微信号'
          clearable
        />
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
import { edit } from '@/api/APP/order'

export default {
  data() {
    return {
      show: false,
      isEdit: false,
      title: '',
      form: {
        loading: false,
        data: {
          rid: '',
          didUserId: '',
          orderType: '',
          name: '',
          phone: '',
          wechat: ''
        },
        rules: {
          rid: [{ required: true, message: '请输入关联编号', trigger: 'blur' }],
          didUserId: [{ required: true, message: '请输入didUserId', trigger: 'blur' }],
          orderType: [{ required: true, message: '请选择订单类型', trigger: 'blur' }],
          name: [{ required: true, message: '请输入用户名称', trigger: 'blur' }],
          phone: [{ required: true, message: '请输入手机号码', trigger: 'blur' }],
          wechat: [{ required: true, message: '请输入微信号', trigger: 'blur' }]
        }
      }
    }
  },
  methods: {
    toggle(show, data) {
      !show && this.reset()
      this.isEdit = !!data
      this.show = show
      !!data && this.$nextTick().then(() => {
        Object.assign(this.form.data, data)
      })
    },
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.form.data.destructionId = null
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
