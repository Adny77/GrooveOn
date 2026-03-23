// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'song_duplicate_check_response.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

SongDuplicateCheckResponse _$SongDuplicateCheckResponseFromJson(
  Map<String, dynamic> json,
) => SongDuplicateCheckResponse(
  existingSongs: (json['existingSongs'] as List<dynamic>)
      .map((e) => ExistingSongInfoResponse.fromJson(e as Map<String, dynamic>))
      .toList(),
  missingExternalTrackIds: (json['missingExternalTrackIds'] as List<dynamic>)
      .map((e) => e as String)
      .toList(),
);

Map<String, dynamic> _$SongDuplicateCheckResponseToJson(
  SongDuplicateCheckResponse instance,
) => <String, dynamic>{
  'existingSongs': instance.existingSongs.map((e) => e.toJson()).toList(),
  'missingExternalTrackIds': instance.missingExternalTrackIds,
};
