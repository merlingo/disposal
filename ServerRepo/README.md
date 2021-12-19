# DataService
Veri yönetimi ve dağıtımından sorumlu ambar modülü
nodejs sunucusu olarak kurulmuştur
conn_str ile bağlantı kurulacak mongodb sunucusu 
app.js main dosyasıdır. bu dosya ilk çalıştırılan js kodlarını içerir
Etkileşim klasörü sunucuya yapılan api isteklerini ve sunucudan veri modeline yapılan crudl isteklerini içerir
web.js sunucuya yapılan get-post-put-delete fonksiyonlarını içermektedir:
	- Her fonksiyonda 3 ana öğe bulunur; field,modeln,type
	- field: işlem yapılacak verinin ait olduğu modül, klasör, alan
	- modeln: field içindeki modelin adı
	- type: işlem türü - m(manipulation), f(funtional), c(config)
crudl.js crudl fonksiyonlarını içermektedir. modelin ambardan çekilmesi ve istenilen işlemi yapılmasına dair fonksiyonları içerir
	- her biri için veri modeli çekilir
	- işlem türüne göre veri manipule edilebilir, fonksiyonel olarak değerlendirilebilir, configürasyon olarak işlem görebilir
	- her model için; alan ve değerleri bazında veri, sahip olduğu fonksiyon, xml olarak paylaşılan configürasyonu vardır.
	- AbsRepos'tan veri modeli getModel fonksiyonu ile alınır.

Veriya ait işlemler DataService klasörü altında gerçekleştirilir.
AbsRepos dosyası veri modelleri için abstract fonksiyonları içerir.
AbsRepos fonksiyonunu extend eden bir sınıf bulunmaktadır. Bu sınıf getModel ve connectDB fonksiyonlarını implement eder
	- getModel fonksiyonu ile ilgili model çağrılmaktadır.
	- Her Alan/modül için bir js dosyası vardır. Modüllerin ortak yanı getModel fonksiyonu içermeleridir.
datamodel veri modellerini içermektedir. Her field için yeni bir datamodel tanımlanmalıdır.
	- Her modelin manipülasyon field alanları bulunmaktadır.
	- fonksiyonlar modeli vardır. Bu model, veri modellerinin fonksiyonlarını yönetmek ve çalıştırmak için kullanılmaktadır
	- Konfigürasyon yönetimi için de bir model bulunmaktadır. Bu model ile hangi veri modeli için ne tür konfig ayarları gerekmekte, config dosyası yazım bilgisi ve paylaşım şekli gibi bilgiler içermektedir
Configs klasöründe repository için config dosyası import fonksiyonu ve parse edilecek değerler yer almaktadır.