const notificationBottom = Swal.mixin({
    toast: true,
    position: 'bottom-end',
    showConfirmButton: false,
    timer: 7500,
    timerProgressBar: true,
    iconColor: 'white',
    customClass: {
      popup: 'colored-toast'
    },
    showClass: {
        popup: 'animate__animated animate__slideInRight'
    },
    hideClass: {
        popup: 'animate__animated animate__slideOutRight'
    },
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer)
      toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
  })