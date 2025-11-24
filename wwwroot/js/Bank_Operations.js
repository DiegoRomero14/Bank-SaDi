document.addEventListener('DOMContentLoaded', function () {

    //Para el modal de deposito (Inicio)
    const depositButton = document.querySelector('#btn-deposit');
    const modalDeposit = document.querySelector('#modal-Deposit');
    const DcancelButton = document.querySelector('#DcancelUpdate');

    if (depositButton && modalDeposit && DcancelButton) {
        depositButton.addEventListener('click', () => {
            document.getElementById("InputDesposit").value = '';
            modalDeposit.classList.add('show');
            modalDeposit.style.display = 'flex';
        });

        DcancelButton.addEventListener('click', () => {
            modalDeposit.style.display = 'none';
        });
    }

    const predefinedDepositButtons = document.querySelectorAll('.modalD-optionValues button');
    predefinedDepositButtons.forEach(button => {
        button.addEventListener('click', function () {
            const value = this.textContent.replace(/\D/g, ''); // Elimina todo excepto los números
            document.getElementById('InputDesposit').value = value;
        });
    });
    //Para el modal de deposito (Fin)


    //Para el modal de retiro (Inicio)
    const withdrawButton = document.querySelector('#btn-withdraw');
    const modalWithdraw = document.querySelector('#modal-Withdraw');
    const WcancelButton = document.querySelector('#WcancelUpdate');

    if (withdrawButton && modalWithdraw && WcancelButton) {
        withdrawButton.addEventListener('click', () => {
            document.getElementById("InputWithdraw").value = '';
            modalWithdraw.classList.add('show');
            modalWithdraw.style.display = 'flex';
        });

        WcancelButton.addEventListener('click', () => {
            modalWithdraw.style.display = 'none';
        });
    }

    const predefinedWithdrawButtons = document.querySelectorAll('.modalW-optionValues button');
    predefinedWithdrawButtons.forEach(button => {
        button.addEventListener('click', function () {
            const value = this.textContent.replace(/\D/g, ''); // Elimina todo excepto los números
            document.getElementById('InputWithdraw').value = value;
        });
    });
    //Para el modal de retiro (Fin)


    //Para el modal de intereses (Inicio)
    const interesButton = document.querySelector('#btn-interes');
    const modalInteres = document.querySelector('#modal-Interes');
    const IcancelButton = document.querySelector('#IcancelUpdate');

    if (interesButton && modalInteres && IcancelButton) {
        interesButton.addEventListener('click', () => {
            modalInteres.classList.add('show');
            modalInteres.style.display = 'flex';
        });

        IcancelButton.addEventListener('click', () => {
            modalInteres.style.display = 'none';
        });
    }
    //Para el modal de intereses (Fin)

    //Para el modal de movevments (Inicio)
    const showmovementsButton = document.querySelector('#btn-movements');
    const modalMovements = document.querySelector('#modal-Movements');
    const McancelButton = document.querySelector('#McancelUpdate');

    if (showmovementsButton && modalMovements && McancelButton) {
        showmovementsButton.addEventListener('click', () => {
            modalMovements.classList.add('show');
            modalMovements.style.display = 'flex';
        });

        McancelButton.addEventListener('click', () => {
            modalMovements.style.display = 'none';
        });
    }
    //Para el modal de movevments (Fin)

    //Para el modal de trasnfer (Inicio)
    const transferButton = document.querySelector('#btn-transfer');
    const modalTransfer = document.querySelector('#modal-Transfer');
    const TcancelButton = document.querySelector('#TcancelUpdate');

    if (transferButton && modalTransfer && TcancelButton) {
        transferButton.addEventListener('click', () => {
            document.getElementById("InputTransferAmount").value = '';
            document.getElementById("InputTransferNumAccount").value = '';
            modalTransfer.classList.add('show');
            modalTransfer.style.display = 'flex';
        });

        TcancelButton.addEventListener('click', () => {
            modalTransfer.style.display = 'none';
        });
    }

    const quickTransferButtons = document.querySelectorAll('.quick-transfer');
    quickTransferButtons.forEach(button => {
        button.onclick = function () {
            const amount = this.getAttribute('data-amount');
            document.getElementById("InputTransferAmount").value = amount;
        };
    });
    //Para el modal de transfer (Fin)



    //Manejo de envio del formulario de deposito
    document.getElementById('formDeposit').addEventListener('submit', function (e) {
        e.preventDefault(); // Evitar el envío normal del formulario

        const formData = new FormData(this); // Obtener datos del formulario

        fetch(this.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest' // Indicar que es una solicitud AJAX
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // Si la respuesta es exitosa, mostrar mensaje y redirigir
                    Swal.fire({
                        title: 'Success!',
                        text: data.message,
                        icon: 'success',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir a la URL proporcionada en la respuesta
                        window.location.href = data.redirectUrl;
                    });
                } else {
                    // Si hay un error, mostrar mensaje de error
                    Swal.fire({
                        title: 'Error!',
                        text: data.message,
                        icon: 'error',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir a la URL proporcionada en la respuesta
                        window.location.href = data.redirectUrl;
                    });
                }
            })
            .catch(error => {
                Swal.fire({
                    title: 'Error!',
                    text: 'An error occurred while processing the request.',
                    icon: 'error',
                    confirmButtonText: 'Accept'
                }).then(() => {
                    // Redirigir a la URL proporcionada en la respuesta
                    window.location.href = data.redirectUrl;
                });
            });
    });

    // Manejo del envío del formulario de retiro
    document.getElementById('formWithdraw').addEventListener('submit', function (e) {
        e.preventDefault(); // Evitar el envío normal del formulario

        const formData = new FormData(this); // Obtener datos del formulario

        fetch(this.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest' // Indicar que es una solicitud AJAX
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    // Si la respuesta es exitosa, mostrar mensaje y redirigir
                    Swal.fire({
                        title: 'Success!',
                        text: data.message,
                        icon: 'success',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir a la URL proporcionada en la respuesta
                        window.location.href = data.redirectUrl;
                    });
                } else {
                    // Si hay un error, mostrar mensaje de error
                    Swal.fire({
                        title: 'Error!',
                        text: data.message,
                        icon: 'error',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir a la URL proporcionada en la respuesta
                        window.location.href = data.redirectUrl;
                    });
                }
            })
            .catch(error => {
                Swal.fire({
                    title: 'Error!',
                    text: 'An error occurred while processing the request.',
                    icon: 'error',
                    confirmButtonText: 'Accept'
                }).then(() => {
                    // Redirigir a la URL proporcionada en la respuesta
                    window.location.href = data.redirectUrl;
                });
            });
    });

    //Manejo de calcular intereses 
    if (interesButton) {
        document.getElementById("btn-interes").addEventListener("click", function () {
            fetch('/Bank_Operations/CalculateInterest')
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        document.getElementById("InputInteres").value = data.interest.toFixed(2); // Muestra el interés calculado
                    } else {
                        alert(data.message); // Muestra un mensaje de error
                    }
                })
                .catch(error => console.error('Error:', error));
        });
    }

    //Manejo de envio de formulario transferencia
    document.getElementById("transferForm").addEventListener('submit', function (e) {
        e.preventDefault(); // Previene el envío del formulario por defecto

        const formData = new FormData(this); // Obtener datos del formulario

        fetch(this.action, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest' // Indicar que es una solicitud AJAX
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json(); // Parsear la respuesta JSON
            })
            .then(data => {
                // Verificar si la respuesta fue exitosa
                if (data.success) {
                    // Mostrar mensaje de éxito
                    Swal.fire({
                        title: 'Success!',
                        text: data.message,
                        icon: 'success',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir si se proporciona una URL
                        if (data.redirectUrl) {
                            window.location.href = data.redirectUrl;
                        }
                    });
                } else {
                    // Mostrar mensaje de error
                    Swal.fire({
                        title: 'Error!',
                        text: data.message,
                        icon: 'error',
                        confirmButtonText: 'Accept'
                    }).then(() => {
                        // Redirigir si se proporciona una URL
                        if (data.redirectUrl) {
                            window.location.href = data.redirectUrl;
                        }
                    });
                }
            })
            .catch(error => {
                console.error('Error:', error);
                // Mostrar mensaje de error en caso de fallo en la solicitud
                Swal.fire({
                    title: 'Error!',
                    text: 'An error occurred while processing the request.',
                    icon: 'error',
                    confirmButtonText: 'Accept'
                });
            });
    });




});

