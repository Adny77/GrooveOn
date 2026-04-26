import 'package:grooveon_mobile/models/playlist_song_response.dart';
import 'base_provider.dart';

class PlaylistSongProvider extends BaseProvider<PlaylistSongResponse> {
  PlaylistSongProvider() : super("PlaylistSong");

  @override
  PlaylistSongResponse fromJson(data) {
    return PlaylistSongResponse.fromJson(data);
  }
}