import 'package:json_annotation/json_annotation.dart';

part 'existing_song_info_response.g.dart';

@JsonSerializable()
class ExistingSongInfoResponse {
  final int id;
  final String? externalTrackId;
  final String title;
  final String artistName;
  final String? albumTitle;
  final String? coverUrl;

  ExistingSongInfoResponse({
    required this.id,
    this.externalTrackId,
    required this.title,
    required this.artistName,
    this.albumTitle,
    this.coverUrl,
  });

  factory ExistingSongInfoResponse.fromJson(Map<String, dynamic> json) =>
      _$ExistingSongInfoResponseFromJson(json);

  Map<String, dynamic> toJson() => _$ExistingSongInfoResponseToJson(this);
}