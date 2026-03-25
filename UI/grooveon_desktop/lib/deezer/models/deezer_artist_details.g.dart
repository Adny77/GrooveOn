// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'deezer_artist_details.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DeezerArtistDetails _$DeezerArtistDetailsFromJson(Map<String, dynamic> json) =>
    DeezerArtistDetails(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
      genres: json['genres'] == null
          ? null
          : DeezerGenresWrapper.fromJson(
              json['genres'] as Map<String, dynamic>,
            ),
    );

Map<String, dynamic> _$DeezerArtistDetailsToJson(
  DeezerArtistDetails instance,
) => <String, dynamic>{
  'id': instance.id,
  'name': instance.name,
  'genres': instance.genres,
};

DeezerGenresWrapper _$DeezerGenresWrapperFromJson(Map<String, dynamic> json) =>
    DeezerGenresWrapper(
      data:
          (json['data'] as List<dynamic>?)
              ?.map(
                (e) => DeezerGenreResponse.fromJson(e as Map<String, dynamic>),
              )
              .toList() ??
          const [],
    );

Map<String, dynamic> _$DeezerGenresWrapperToJson(
  DeezerGenresWrapper instance,
) => <String, dynamic>{'data': instance.data};
