function SearchZipCode() {
    $(document).ready(function () {

        function clear_fields() {
            $("#Address_Street").val("");
            $("#Address_Neighborhood").val("");
            $("#Address_City").val("");
            $("#Address_State").val("");
        }

        $("#Address_ZipCode").blur(function () {
            var zipcode = $(this).val().replace(/\D/g, '');

            if (zipcode != "") {
                var checkzipcode = /^[0-9]{8}$/;

                if (checkzipcode.test(zipcode)) {
                    $("#Address_Street").val("...");
                    $("#Address_Neighborhood").val("...");
                    $("#Address_City").val("...");
                    $("#Address_State").val("...");

                    $.getJSON("https://viacep.com.br/ws/" + zipcode + "/json/?callback=?",
                        function (data) {
                            if (!("erro" in data)) {
                                $("#Address_Street").val(data.logradouro);
                                $("#Address_Neighborhood").val(data.bairro);
                                $("#Address_City").val(data.localidade);
                                $("#Address_State").val(data.uf);
                            }
                            else {
                                clear_fields();
                                alert("CEP não encontrado.");
                            }
                        });
                }
                else {
                    clear_fields();
                    alert("Formato de CEP inválido.");
                }
            }
            else {
                clear_fields();
            }
        });
    });
}