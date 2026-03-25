import 'package:grooveon_desktop/models/response/genre_response.dart';
import 'base_provider.dart';

class GenreProvider extends BaseProvider<GenreResponse> {
  GenreProvider() : super("Genre");

  @override
  GenreResponse fromJson(data) {
    return GenreResponse.fromJson(data);
  }
}