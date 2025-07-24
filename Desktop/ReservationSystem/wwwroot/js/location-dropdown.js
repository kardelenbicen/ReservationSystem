const cityDistricts = {
    "İstanbul": ["Kadıköy", "Beşiktaş", "Üsküdar", "Bakırköy"],
    "Ankara": ["Çankaya", "Keçiören", "Yenimahalle", "Mamak"],
    "İzmir": ["Konak", "Karşıyaka", "Bornova", "Buca"]
};

$(document).ready(function () {
    $('#citySelect').on('change', function () {
        const selectedCity = $(this).val();
        const $districtSelect = $('#districtSelect');
        $districtSelect.empty();

        if (selectedCity && cityDistricts[selectedCity]) {
            $.each(cityDistricts[selectedCity], function (index, district) {
                $districtSelect.append($('<option>', {
                    value: district,
                    text: district
                }));
            });
        } else {
            $districtSelect.append($('<option>', {
                value: '',
                text: 'İl seçiniz'
            }));
        }
    });

    $('#citySelect').trigger('change');
});
