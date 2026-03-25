import 'package:grooveon_desktop/deezer/models/deezer_genre_response.dart';
import 'package:json_annotation/json_annotation.dart';

part 'deezer_artist_details.g.dart';

@JsonSerializable()
class DeezerArtistDetails {
  final int id;
  final String name;
  final DeezerGenresWrapper? genres;

  DeezerArtistDetails({
    required this.id,
    required this.name,
    this.genres,
  });

  factory DeezerArtistDetails.fromJson(Map<String, dynamic> json) =>
      _$DeezerArtistDetailsFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerArtistDetailsToJson(this);
}



@JsonSerializable()
class DeezerGenresWrapper {
  final List<DeezerGenreResponse> data;

  DeezerGenresWrapper({
    this.data = const [],
  });

  factory DeezerGenresWrapper.fromJson(Map<String, dynamic> json) =>
      _$DeezerGenresWrapperFromJson(json);

  Map<String, dynamic> toJson() => _$DeezerGenresWrapperToJson(this);
}