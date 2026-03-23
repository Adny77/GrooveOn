// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'existing_album_track_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ExistingAlbumTrackResponse _$ExistingAlbumTrackResponseFromJson(
  Map<String, dynamic> json,
) => ExistingAlbumTrackResponse(
  externalTrackId: json['externalTrackId'] as String,
  title: json['title'] as String,
  alreadyExists: json['alreadyExists'] as bool,
);

Map<String, dynamic> _$ExistingAlbumTrackResponseToJson(
  ExistingAlbumTrackResponse instance,
) => <String, dynamic>{
  'externalTrackId': instance.externalTrackId,
  'title': instance.title,
  'alreadyExists': instance.alreadyExists,
};
