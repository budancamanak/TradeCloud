import {toast} from "react-toastify"

export const ToastConfigs = Object.freeze({
    info: {
        type: "info",
        position: "bottom-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        newestOnTop: true,
    },
    success: {
        type: "success",
        position: "bottom-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
    },
    error: {
        type: "error",
        position: "bottom-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        newestOnTop: true,
    },
    warning: {
        type: "warning",
        position: "bottom-right",
        autoClose: 3000,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
        newestOnTop: true,
    },
    login: {
        type: "success",
        position: "bottom-right",
        autoClose: 1500,
        hideProgressBar: false,
        closeOnClick: false,
        pauseOnHover: true,
        draggable: true,
        progress: undefined,
    },
})

export const ToastUtility = Object.freeze({
    default: function (message, toastId) {
        return toast(message, {...ToastConfigs.default, toastId: toastId})
    },

    info: function (message, toastId) {
        return toast(message, {...ToastConfigs.info, toastId: toastId})
    },
    success: function (message, toastId) {
        return toast(message, {...ToastConfigs.success, toastId: toastId})
    },
    login: function (message) {
        return toast(message, {...ToastConfigs.login})
    },
    error: function (message, toastId) {
        return toast(message, {...ToastConfigs.error, toastId: toastId})
    },
    warning: function (message, toastId) {
        return toast(message, {...ToastConfigs.warning, toastId: toastId})
    },
    dismiss: function (toastId) {
        toast.dismiss(toastId)
    },
    update: function (toastId, configToUpdate, message) {
        if (!toast.isActive(toastId)) {
            toast(message, {...configToUpdate, toastId: toastId, render: message})
        } else {
            toast.update(toastId, {...configToUpdate, toastId: toastId, render: message})
        }
    },
    isActive: function (toastId) {
        return toast.isActive(toastId)
    },
})
