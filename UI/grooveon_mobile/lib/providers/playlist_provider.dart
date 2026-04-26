import 'package:grooveon_mobile/models/playlist_response.dart';
import 'base_provider.dart';

class PlaylistProvider extends BaseProvider<PlaylistResponse> {
  PlaylistProvider() : super("Playlist");

  @override
  PlaylistResponse fromJson(data) {
    return PlaylistResponse.fromJson(data);
  }
}