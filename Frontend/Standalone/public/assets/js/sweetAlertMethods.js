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
        popup: 'animate__animated animate__fadeInRight'
    },
    hideClass: {
        popup: 'animate__animated animate__fadeOutRight'
    },
    didOpen: (toast) => {
      toast.addEventListener('mouseenter', Swal.stopTimer)
      toast.addEventListener('mouseleave', Swal.resumeTimer)
      toast.addEventListener('click', Swal.close)
    }
  })

  const settings = Swal.mixin({
    toast: false,
    position: 'center',
    showConfirmButton: true,
    iconColor: 'white',
    customClass: {
      popup: 'dark-bg'
    },
    showClass: {
        popup: 'animate__animated animate__fadeInRight'
    },
    hideClass: {
        popup: 'animate__animated animate__fadeOutRight'
    },
    didOpen: (toast) => {
    }
  })