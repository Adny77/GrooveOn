// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'album_preview_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AlbumPreviewResponse _$AlbumPreviewResponseFromJson(
  Map<String, dynamic> json,
) => AlbumPreviewResponse(
  albumAlreadyExists: json['albumAlreadyExists'] as bool,
  tracks: (json['tracks'] as List<dynamic>)
      .map(
        (e) => ExistingAlbumTrackResponse.fromJson(e as Map<String, dynamic>),
      )
      .toList(),
  existingTracksCount: (json['existingTracksCount'] as num).toInt(),
  newTracksCount: (json['newTracksCount'] as num).toInt(),
);

Map<String, dynamic> _$AlbumPreviewResponseToJson(
  AlbumPreviewResponse instance,
) => <String, dynamic>{
  'albumAlreadyExists': instance.albumAlreadyExists,
  'tracks': instance.tracks.map((e) => e.toJson()).toList(),
  'existingTracksCount': instance.existingTracksCount,
  'newTracksCount': instance.newTracksCount,
};
