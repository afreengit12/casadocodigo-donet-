class Carrinho {
    clickIncremento(button) {
        let data = this.getData(button);
        data.Quantidade++;
        this.postQuantidade(data);
    }

    clickDecremento(button) {
        let data = this.getData(button);
        data.Quantidade--;
        this.postQuantidade(data);
    }

    updateQuantidade(input) {
        let data = this.getData(input);
        this.postQuantidade(data);
    }

    getData(elemento) {
        var linhaItem = $(elemento).parents('[item-id]'); // A partir do botão, varre os elementos acima (pais), buscando pelo atributo 'item-id', pegando assim a linha desejada.
        var itemId = $(linhaItem).attr('item-id'); // Localizada a linha desejada, pega o valor do atributo 'item-id', atribuindo à variável 'itemId'.
        var novaQuantidade = $(linhaItem).find('input').val();

        return {
            Id: itemId,
            Quantidade: novaQuantidade
        };
    }

    postQuantidade(data) {
        let token = $('[name=__RequestVerificationToken]').val();
        let headers = {};
        headers["RequestVerificationToken"] = token;

        $.ajax({
            url: "/pedido/updatequantidade",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(data),
            headers: headers
        }).done(function (response) {
            let itemPedido = response.itemPedido;
            let carrinhoViewModel = response.carrinhoViewModel;

            let linhaDoItem = $('[item-id=' + response.itemPedido.id + ']');
            $(linhaDoItem).find('input').val(itemPedido.quantidade);
            $(linhaDoItem).find('[subtotal]').html((itemPedido.subtotal).duasCasas());

            $('[numero-itens]').html('Total: ' + carrinhoViewModel.itens.length + ' itens');
            $('[total]').html((carrinhoViewModel.total).duasCasas());

            if (itemPedido.quantidade == 0)
                $(linhaDoItem).remove();

            if (carrinhoViewModel.itens.length == 0) {
                $('[no-itens]').html('Carrinho Vazio');
                $('[btnFinalizarPedido]').attr('disabled', true);
                $('[btnFinalizarPedido]').on('click', function (e) {
                    e.preventDefault();
                });
            }

            console.log(JSON.stringify(itemPedido));
            console.log(JSON.stringify(carrinhoViewModel));
        });
    }
}

var carrinho = new Carrinho();

Number.prototype.duasCasas = function () {
    return this.toFixed(2).replace('.', ',');
}