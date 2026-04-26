// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'playlist_song_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

PlaylistSongResponse _$PlaylistSongResponseFromJson(
  Map<String, dynamic> json,
) => PlaylistSongResponse(
  id: (json['id'] as num).toInt(),
  playlistId: (json['playlistId'] as num).toInt(),
  playlistName: json['playlistName'] as String?,
  songId: (json['songId'] as num).toInt(),
  songTitle: json['songTitle'] as String?,
  artistName: json['artistName'] as String?,
  coverUrl: json['coverUrl'] as String?,
  externalTrackId: json['externalTrackId'] as String?,
  durationSeconds: (json['durationSeconds'] as num?)?.toInt(),
  addedAt: DateTime.parse(json['addedAt'] as String),
);

Map<String, dynamic> _$PlaylistSongResponseToJson(
  PlaylistSongResponse instance,
) => <String, dynamic>{
  'id': instance.id,
  'playlistId': instance.playlistId,
  'playlistName': instance.playlistName,
  'songId': instance.songId,
  'songTitle': instance.songTitle,
  'artistName': instance.artistName,
  'coverUrl': instance.coverUrl,
  'externalTrackId': instance.externalTrackId,
  'durationSeconds': instance.durationSeconds,
  'addedAt': instance.addedAt.toIso8601String(),
};
