import 'package:grooveon_desktop/models/response/artist_response.dart';
import 'base_provider.dart';

class ArtistProvider extends BaseProvider<ArtistResponse> {
  ArtistProvider() : super("Artist");

  @override
  ArtistResponse fromJson(data) {
    return ArtistResponse.fromJson(data);
  }
}
